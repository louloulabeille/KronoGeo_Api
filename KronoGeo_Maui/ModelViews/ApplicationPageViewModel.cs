using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.Platforms.Android.Application.Geolocalisation;
using Microsoft.Extensions.Primitives;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Google.Crypto.Tink.Signature;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using Task = System.Threading.Tasks.Task;

namespace KronoGeo_Maui.ModelViews
{
    public partial class ApplicationPageViewModel : ObservableObject, IRecipient<LocationChangedMessage>
    {
        #region private readonly properties

        private readonly IServiceGeolocalisation _service;
        private readonly List<Localisation> _localisations;
        private readonly IServiceSaveLocalisation _saveLocalisation;
#endregion

        #region constructeur
        public ApplicationPageViewModel(IServiceGeolocalisation service
            , IServiceSaveLocalisation saveLocalisation)
        {
            // -- garde le service
            _service = service;
#if !ANDROID
            _service.LocationChanged += OnLocalication_Changed;
#endif
            _localisations = [];
            _saveLocalisation = saveLocalisation;

            Task.Run(async () => await GetUserLocationAsync());
        }
#endregion

        #region public properties
        public bool IsMessageError { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        /// <summary>
        /// pour savoir si le service est en pause ou non
        /// </summary>
        public bool IsPause { get; set; } = false;
        public bool IsStart { get; set; } = false;
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
#if ANDROID
                var status = await Permissions.CheckStatusAsync<Permissions.PostNotifications>();
                if (status != PermissionStatus.Granted)
                {
                    throw new PermissionException("Permission pour les notifications non donnée.");
                }

                var intent = new Intent(Android.App.Application.Context, typeof(GeoAndroidService));
                
                if ( IsPause && IsStart )   // -- en pause et le service a démarré
                {
                    intent.SetAction(GeoAndroidService.ActionStopPause);
                    IsPause = false;
                    PlayPause = "\ue1a2";
                }
                else if (!IsPause && IsStart)
                {
                    intent.SetAction(GeoAndroidService.ActionPause);
                    IsPause = true;
                    PlayPause = "\ue1c4";
                }

                if (!IsStart)
                {
                    IsStart = true;
                    PlayPause = "\ue1a2";
                    intent.SetAction(GeoAndroidService.ActionStart);
                    if (OperatingSystem.IsAndroidVersionAtLeast(26))
                    {
                        Android.App.Application.Context.StartForegroundService(intent);
                    }
                    else
                    {
                        Android.App.Application.Context.StartService(intent);
                    }
                }
                
#endif

#if !ANDROID
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
#endif
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
                    IsStart = false;
                    IsPause = false;
                    PlayPause = "\ue1c4";
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
#if ANDROID
                var intent = new Intent(Android.App.Application.Context, typeof(GeoAndroidService));
                intent.SetAction(GeoAndroidService.ActionStop);
                if (OperatingSystem.IsAndroidVersionAtLeast(26))
                {
                    Android.App.Application.Context.StartForegroundService(intent);
                }
                else
                {
                    Android.App.Application.Context.StartService(intent);
                }
#endif

#if !ANDROID
                _service.StopLocationUpdates();
                
#endif
                if (_localisations.Count > 0)
                {
                    await _saveLocalisation.SaveLocalisation(_localisations, new System.Threading.CancellationToken());
                    _localisations.Clear();
                }
                PlayPause = "\ue1c4";
                IsStart = false;
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
            TraitementLocalisation(message.Value);
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
            TraitementLocalisation(e.Location);
        }

        #endregion

        #region private method
        /// <summary>
        /// Method de traitement de la localisation
        /// </summary>
        /// <param name="location"></param>
        private void TraitementLocalisation (Location location)
        {
            Location = location; // -- pour la mise a jour du tracé sur la map
            // -- envoie un message pour recentrer la map sur la position de l'utilisateur
            WeakReferenceMessenger.Default.Send(new RecenterMapMessage(location));
            // -- ajoute la localisation dans la liste pour l'enregistrement
            _localisations.Add(new Localisation()
            {
                Altitude = location.Altitude,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Accuracy = location.Accuracy,
                Speed = location.Speed,
                Timestamp = DateTime.Now,   // -- mettre le DateTime local sinon universel
                VerticalAccuracy = location.VerticalAccuracy,
                Course = location.Course,
            });
        } 
        #endregion
    }
}
