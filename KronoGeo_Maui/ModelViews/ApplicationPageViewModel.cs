#if ANDROID
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using KronoGeo_Maui.Platforms.Android.Application.Geolocalisation;
using Xamarin.Google.Crypto.Tink.Signature;
#endif
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
using System.Collections.ObjectModel;

namespace KronoGeo_Maui.ModelViews
{
    /// <summary>
    /// class de base 
    /// </summary>
    public abstract class PageBaseViewModel { }

    /// <summary>
    /// class MapViewModel et CameraViewModel rajouter les autres classes par la suite
    /// </summary>
    public class MapViewModel : PageBaseViewModel { }
    public class CameraViewModel : PageBaseViewModel { }

    public partial class ApplicationPageViewModel : ObservableObject, IRecipient<LocationChangedMessage>, IDisposable
    {
        #region private readonly properties
        private readonly IServiceGeolocalisation _service;
        private readonly List<Localisation> _localisations;
        private readonly IServiceSaveLocalisation _saveLocalisation;
        #endregion

        #region private properties
        private Location? _lastLocation { get; set; } = default;
        private double _distance { get; set; } = 0;
        #endregion

        #region public properties
        public ObservableCollection<PageBaseViewModel> MesPages { get; set; }

        #endregion

        #region constructeur
        public ApplicationPageViewModel(IServiceGeolocalisation service
            , IServiceSaveLocalisation saveLocalisation)
        {
            MesPages = [];
            MesPages.Add(new MapViewModel());
            MesPages.Add(new CameraViewModel());
            // -- garde le service
            _service = service;
#if !ANDROID
            _service.LocationChanged += OnLocalication_Changed;
#endif
            _localisations = [];
            _saveLocalisation = saveLocalisation;
            // -- enregistrer dans le registre des messages pour recevoir les messages de type LocationChangedMessage
            //WeakReferenceMessenger.Default.RegisterAll(this);
            WeakReferenceMessenger.Default.Register<LocationChangedMessage>(this);

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
        [ObservableProperty]
        public partial string MessageDistance { get; set; } = string.Empty;
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

        /// <summary>
        /// démarre ou met en pause le service de géolocalisation
        /// </summary>
        /// <returns></returns>
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
                    IsPause = false;
                    PlayPause = "\ue1a2";
                    intent.SetAction(GeoAndroidService.ActionStopPause);
                    Android.App.Application.Context.StartService(intent);
                }
                else if (!IsPause && IsStart)
                {
                    IsPause = true;
                    PlayPause = "\ue1c4";
                    intent.SetAction(GeoAndroidService.ActionPause);
                    Android.App.Application.Context.StartService(intent);
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

        /// <summary>
        /// arrête le service de géolocalisation et enregistre les localisations dans la base de données
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task Stop()
        {
            try
            {
#if ANDROID
                // -- création d'un service pour marcher en arrière plan
                var intent = new Intent(Android.App.Application.Context, typeof(GeoAndroidService));
                intent.SetAction(GeoAndroidService.ActionStop);
                Android.App.Application.Context.StartService(intent);
#endif

#if !ANDROID
                _service.StopLocationUpdates();
                
#endif
                if (_localisations.Count > 0)
                {
                    if ( await _saveLocalisation.SaveLocalisation(_localisations, new System.Threading.CancellationToken()))
                    { // -- enregistrement ok
                        _localisations.Clear();
                        // -- initalisation de la map au niveau de Polyne
                        WeakReferenceMessenger.Default.Send(new PolyneMapMessage(null));
                    }
                }
                // -- initialisation de la map sur la position de l'utilisateur
                await Task.Run(async () => await GetUserLocationAsync());

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
            // -- faire le traitement dans le thread principal pour éviter les erreurs de cross thread
            MainThread.BeginInvokeOnMainThread(() =>
            {
                TraitementLocalisation(message.Value);
            });
        }
        #endregion

        #region public method IDisposable
        /// <summary>
        /// Dispose method to unregister from the WeakReferenceMessenger and suppress finalization.
        /// </summary>
        public void Dispose()
        {
            WeakReferenceMessenger.Default.Unregister<LocationChangedMessage>(this);
            GC.SuppressFinalize(this);
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
            Localisation localisation = new()
            {
                OrderIndex = 0,
                Altitude = location.Altitude,
                Latitude = location.Latitude,
                Longitude = location.Longitude,
                Accuracy = location.Accuracy,
                Speed = location.Speed,
                Timestamp = DateTimeOffset.Now,   // -- mettre le DateTime local sinon universel
                VerticalAccuracy = location.VerticalAccuracy,
                Course = location.Course,
            };

            // -- y a des doublons au niveau de eventchange Google lors de la mise a jour de la position, donc on ne garde que les nouvelles positions
            if ( !_localisations.Exists(l=> l.Longitude == localisation.Longitude && l.Latitude == localisation.Latitude) )
            {
                if (_lastLocation is not null)
                {
                    // -- calcul de la distance entre la dernière position et la nouvelle
                    _distance += _lastLocation.CalculateDistance(location, DistanceUnits.Kilometers);
                    MessageDistance = $"{_distance} km";
                }
                _lastLocation = location; // -- pour la mise a jour du dernier point pour le calcul de la distance
                // -- envoie un message pour recentrer la map sur la position de l'utilisateur
                //WeakReferenceMessenger.Default.Send(new RecenterMapMessage(location));
                // -- ajoute la localisation dans la liste pour l'enregistrement
                int index = _localisations.Count;
                localisation.OrderIndex = index; // -- mise a jour de l'index
                _localisations.Add(localisation);
                // -- envoie un message pour mettre à jour le tracé sur la map
                WeakReferenceMessenger.Default.Send(new PolyneMapMessage(location));
                
            }
        }

        #endregion
    }
}
