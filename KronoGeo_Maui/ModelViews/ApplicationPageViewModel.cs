using Android.Gms.Tasks;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using Microsoft.Extensions.Primitives;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using Task = System.Threading.Tasks.Task;

namespace KronoGeo_Maui.ModelViews
{
    public partial class ApplicationPageViewModel : ObservableObject, IRecipient<LocationChangedMessage>
    {
        #region private readonly properties
        private readonly IServiceGeolocalisation _service;
        // -- mettre le service d'enregistrement
        //private readonly List<Location> _locations;
        private readonly List<Localisation> _localisations;
        private readonly IServiceSaveLocalisation _saveLocalisation;
        #endregion

        #region constructeur
        public ApplicationPageViewModel(IServiceGeolocalisation service
            , IServiceSaveLocalisation saveLocalisation)
        {
            _service = service;
            _localisations = [];
            _saveLocalisation = saveLocalisation;
            _service.LocationChanged += OnLocalication_Changed;

            Task.Run(async () => await GetUserLocationAsync());
        }
        #endregion

        #region public properties
        public bool IsMessageError { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        #endregion

        #region public propeties ObservableProperties
        [ObservableProperty]
        public partial string PlayPause { get; set; } = "\ue1c4"; // - affichage de play
        /*[ObservableProperty]
        public partial MapSpan? MapRegion { get; set; } = null;*/
        [ObservableProperty]
        public partial Location? Location { get; set; } = default;
        #endregion


        #region method RelayCommand
        [RelayCommand]
        public static async Task ToolbarItem()
        {
            await Shell.Current.GoToAsync("ParametragePage");
        }

        /*[RelayCommand]
        // -- se déclanche après l'affichage de la page Appearing
        public async Task AppearingExe(BindableObject bind)
        {
            
        }*/

        /*[RelayCommand]
        // -- se déclanche après le chagement de la page
        public async Task LoadedExe()
        {
            await GetUserLocationAsync();
        }*/

        [RelayCommand]
        public async Task StartPause()
        {
            IsMessageError = false;
            try
            {
                if (PlayPause == "\ue1c4")
                {
                    _service.Pause = true;
                    PlayPause = "\ue1a2";
                }
                else
                {
                    _service.Pause = false;
                    PlayPause = "\ue1c4";
                }

                _service.StartLocationUpdatesAsync();
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                IsMessageError = true;
                Message = "Votre matériel n'est pas supporté.";
                Console.WriteLine(fnsEx.Message);
            }
            catch (FeatureNotEnabledException fneEx)
            {
                IsMessageError = true;
                Message = "La géolocalisation n'est pas activée. Veuillez l'activer.";
                Console.WriteLine(fneEx.Message);
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                IsMessageError = true;
                Message = "La permission pour la géolocalisation n'a pas été donnée.";
                Console.WriteLine(pEx.Message);
            }
            catch (Exception ex)
            {
                // Unable to get location
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if ( IsMessageError)
                {
                    var cancellationToken = new System.Threading.CancellationToken();
                    await Toast.Make($"{Message}").Show(cancellationToken);
                }
            }
        }

        [RelayCommand]
        public async Task Stop()
        {
            try
            {
                _service.StopLocationUpdates();
                if ( _localisations.Count > 0 )
                {
                    await _saveLocalisation.SaveLocalisation(_localisations, new System.Threading.CancellationToken());
                    _localisations.Clear();
                }
            }
            catch(Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
            }
            finally
            {
                if (IsMessageError)
                {
                    var cancellationToken = new System.Threading.CancellationToken();
                    await Toast.Make($"{Message}").Show(cancellationToken);
                }
            }
        }

        /// <summary>
        /// remplace la localication par défaut par la localication réel au niveau de la map
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetUserLocationAsync()
        {
            var localition = await _service.GetCurrentLocationAsync(new CancellationTokenSource());
            if (localition is not null)
            {
                //MapRegion = MapSpan.FromCenterAndRadius(localition, Distance.FromMeters(500));
                // -- envoie un message pour recentrer la map sur la position de l'utilisateur
                WeakReferenceMessenger.Default.Send(new RecenterMapMessage(localition));
            }

        }

        #endregion

        #region public method interface IRecipient<LocationChangedMessage>
        /// <summary>
        /// method qui est appelé lors d'un send sur le message LocationChangedMessage
        /// il est en écoute
        /// </summary>
        /// <param name="message"></param>
        /// <exception cref="NotImplementedException"></exception>
        public void Receive(LocationChangedMessage message)
        {
            Location = message.Value; // -- pour la mise a jour du tracé sur la map
            WeakReferenceMessenger.Default.Send(new RecenterMapMessage(message.Value));
            _localisations.Add(new Localisation()
            {
                Altitude = message.Value.Altitude,
                Latitude = message.Value.Latitude,
                Longitude = message.Value.Longitude,
                Accuracy = message.Value.Accuracy,
                Speed = message.Value.Speed,
                Timestamp = DateTime.Now,   // -- mettre le DateTime local sinon universel
                VerticalAccuracy = message.Value.VerticalAccuracy,
                Course = message.Value.Course,
            });
        }
        #endregion

        #region public method event
        /// <summary>
        /// évènement pour 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLocalication_Changed(object? sender, GeolocationLocationChangedEventArgs e)
        {
            //_locations.Add(e.Location);

            Location = e.Location; // -- pour la mise a jour du tracé sur la map
            WeakReferenceMessenger.Default.Send(new RecenterMapMessage(e.Location));
            _localisations.Add(new Localisation()
            {
                Altitude = e.Location.Altitude,
                Latitude = e.Location.Latitude,
                Longitude = e.Location.Longitude,
                Accuracy = e.Location.Accuracy,
                Speed = e.Location.Speed,
                Timestamp = DateTime.Now,   // -- mettre le DateTime local sinon universel
                VerticalAccuracy = e.Location.VerticalAccuracy,
                Course = e.Location.Course,
            });
            
        }

        #endregion
    }
}
