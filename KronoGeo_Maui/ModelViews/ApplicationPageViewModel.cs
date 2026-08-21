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
using KronoGeo_Api.Models.Carousel;
using System.Diagnostics;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Maui.Controls.Maps;
using KronoGeo_Maui.Applications.Models;
using CommunityToolkit.Maui;
using CancellationToken = System.Threading.CancellationToken;
using Microsoft.Maui.Controls.Shapes;
using KronoGeo_Maui.PageHelpers;
using CommunityToolkit.Maui.Core;
using KronoGeo_Maui.BottomSheets;
using KronoGeo_Maui.ModelViews.BottomSheets;
using KronoGeo_Api.Models.ModelEventArgs;

namespace KronoGeo_Maui.ModelViews
{
    enum MapTypeEnum
    {
        Street,
        Satellite,
        Hybrid
    }
    public partial class ApplicationPageViewModel : ObservableObject, IRecipient<LocationChangedMessage>, IDisposable
    {
        #region private readonly properties
        private readonly IServiceGeolocalisation _serviceGeo;
        private readonly List<Localisation> _localisations;
        private readonly IServiceSaveLocalisation _saveLocalisation;
        private readonly IServiceCamera _camera;
        private readonly IDialogService _dialogService;
        private readonly IServiceSaveUser _serviceSaveUser;
        private readonly IServiceTelemetry _serviceTelemetry;
        private readonly IServiceBackupGps _serviceBackupGps;
        private readonly IServiceProvider _serviceProvider;
        #endregion

        #region private properties
        private Localisation? _lastLocation { get; set; } = default;
        private RouteTelemetry _routeTelemetry { get; set; }
        private DateTimeOffset _startPauseTime { get; set; }
        private bool _takePhoto = false;
        #endregion

        #region public properties
        //public ObservableCollection<PageBaseViewModel> MesPages { get; set; }
        public ApplicationBottomSheetViewModel SheetViewModel { get; }
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
        /// <summary>
        /// pour savoir si le service est en pause ou non
        /// par rapport à l'affichage de l'icone play ou pause
        /// </summary>
        [ObservableProperty]
        public partial string PlayPause { get; set; } = "\ue1c4"; // - affichage de play 
        /// <summary>
        /// observable collection pour l'affichage des photos dans le carousel
        /// </summary>
        [ObservableProperty]
        public partial ObservableCollection<PhotoDTO> MesPhotos { get; set; } = [];
        /// <summary>
        /// pour savoir si l'affichage de la map est en mode Street ou Satellite ou hybride
        /// </summary>
        [ObservableProperty]
        public partial string MapType { get; set; } = "Street";
        /// <summary>
        /// Desactive la possibilité de prendre des photos 
        /// si le service de géolocalisation n'est pas démarré
        /// </summary>
        [ObservableProperty]
        public partial bool IsEnablePhoto { get; set; } = false;
        #endregion

        #region constructeur
        public ApplicationPageViewModel(IServiceGeolocalisation service
            , IServiceSaveLocalisation saveLocalisation, IServiceCamera camera
            , IDialogService dialogService, IServiceSaveUser serviceSaveUser
            , IServiceTelemetry serviceTelemetry, IServiceBackupGps serviceBackupGps
            , ApplicationBottomSheetViewModel sheetViewModel
            ,IServiceProvider serviceProvider)
        {
            // -- pour affichage des différentes pages du carousel
            /*MesPages = [];
            //MesPages.Add(new MapViewModel());
            MesPages.Add(new ListImageViewModel());
            MesPages.Add(new ResumeViewModel());*/

            // -- chargement des services
            _serviceGeo = service;
            _camera = camera;
            _dialogService = dialogService;
            _serviceSaveUser = serviceSaveUser;
            _serviceTelemetry = serviceTelemetry;
            _serviceBackupGps = serviceBackupGps;
            _serviceProvider = serviceProvider;
#if !ANDROID
            _serviceGeo.LocationChanged += OnLocalication_Changed;
#endif

            _localisations = [];
            _saveLocalisation = saveLocalisation;
            // -- initialise l'object télémétrie
            _routeTelemetry = _serviceTelemetry.CalculateTelemetry(_localisations);

            // -- chargement du view model ApplicationBottomSheetViewModel
            SheetViewModel = sheetViewModel;
            SheetViewModel.DeletePhoto += DeletePhoto;

            // -- enregistrer dans le registre des messages pour recevoir les messages de type LocationChangedMessage
            //WeakReferenceMessenger.Default.RegisterAll(this);
            WeakReferenceMessenger.Default.Register<LocationChangedMessage>(this);

            // -- supprime les photos en local
            // _camera.DeletePhotos();

        }
        #endregion

        #region behaviors
        /// <summary>
        /// method appeler au chargement de la fenêtre
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task AppearingExe() {
            // -- lancement de l'écoute sur les eventHandlers pour sauvegarde des en cas de fermetures accidentel
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window is not null)
            {
                window.Stopped += SaveLocalisation;
                window.Destroying += DestroyingSaveLocalisation;
            }

        }

        /// <summary>
        /// lancement après le chargement de la fenêtre
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task LoadedExe()
        {
            // -- mise a jour dans la Map de la geolocalisation sinon il affiche la map par défaut
            await GetUserLocationAsync();

            // - chargement du popup si un backup existe pour recharger les points dedans
            if (_serviceBackupGps.FileExist())
            {
                var popup = new PopupRechargementPage();
                var result = await _dialogService.ShowPopupAsync<string>(popup, new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = false,
                }, new CancellationToken());
                // - traitement du résultat
                if ( result is not null && bool.Parse(result) )
                {   // -- recharge les données
                    var localisations = _serviceBackupGps.ReturnLocalisation();
                    if ( localisations is not null && localisations.Count > 0)
                    {
                        _localisations.AddRange(localisations.OrderBy(ob => ob.OrderIndex));
                        _lastLocation = _localisations.OrderByDescending(ob => ob.OrderIndex).FirstOrDefault();
                        _routeTelemetry = _serviceTelemetry.CalculateTelemetry(_localisations);

                        // -- passage de la RouteTelemetry vers le BottomSheet
                        SheetViewModel.GetRouteTeletry(_routeTelemetry);

                        // -- supprimer en mémoire les photos
                        SheetViewModel.ClearAllPhotos();

                        // -- mise a jour de la carte
                        foreach (var item in localisations) {
                            var location = item.GetLocation();
                            if (item is LocalisationPhoto photo)
                            {
                                // -- chargement des photos dans le carroussel correspondant 
                                // -- du BottomSheet
                                SheetViewModel.MiseAjourPhoto(new()
                                {
                                    Name = photo.Name,
                                    PathPhoto = photo.PathPhoto
                                });
                                /*MesPhotos.Add(new() 
                                { 
                                    Name = photo.Name, PathPhoto = photo.PathPhoto 
                                });*/
                                PinMessage pinMessage = new()
                                {
                                    Pin = new()
                                    {
                                        Label = $"Photo: {photo.Name}",
                                        Address = $"Position : latitude {location.Latitude}, longitude {location.Longitude}",
                                        Type = PinType.SavedPin,
                                        Location = location,
                                    },
                                    IsAdded = true
                                };
                                // -- envoie un message pour ajouter un pin sur la map
                                WeakReferenceMessenger.Default.Send(new PinMapMessage(pinMessage));
                            }
                            else
                            {
                                // -- envoie un message pour mettre à jour le tracé sur la map
                                WeakReferenceMessenger.Default.Send(new PolyneMapMessage(location));
                            }
                        };
                        _serviceBackupGps.DeleteFile();
                    }
                }
                else
                {
                    _serviceBackupGps.DeleteFile();
                    // -- supprime toutes les photos du BottomSheet
                    SheetViewModel.DeleteAllPhotos();
                    //_camera.DeletePhotos();
                }
            }
        }

        /// <summary>
        /// method appeler à la fermeture la fenêtre 
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task DisappearingExe()
        {
            // -- désabonnement des events
            var window = Application.Current?.Windows.FirstOrDefault();
            if (window is not null)
            {
                window.Stopped -= SaveLocalisation; // -- quand l'application perd le focus ou passe en arrière plan
                window.Destroying -= DestroyingSaveLocalisation;
                SheetViewModel.DeletePhoto -= DeletePhoto;
            }
        }
        #endregion

        #region method RelayCommand
        [RelayCommand]
        public async Task OpenButtomSheet()
        {
            // -- ouverture du bottomsheetMEs
            var sheet = _serviceProvider.GetRequiredService<ApplicationBottomSheet>();
            await sheet.ShowAsync();
        }

        /// <summary>
        /// ouvre la page de paramétrage
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public static async Task BarItemParam()
        {
            await Shell.Current.GoToAsync("ParametragePage");
        }

        [RelayCommand]
        public static async Task BarItemClose()
        {
            System.Environment.Exit(0);
        }

        /// <summary>
        /// method de prise de photo et enregistrement de la localisation
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task TakePhoto()
        {
            IsMessageError = false;
            try
            {
                _takePhoto = true;
                var photo = await _camera.TakePhotoAsync();
                if (photo is not null && !string.IsNullOrEmpty(photo.Name) )
                {
                    //MesPhotos.Add(photo);
                    // -- ajoute la photo dans le BottomSheet
                    SheetViewModel.MiseAjourPhoto(photo);

                    var location = await _serviceGeo.GetCurrentLocationAsync(new CancellationTokenSource().Token);
                    if (location is not null )
                    {
                        PinMessage pinMessage = new()
                        {
                            Pin = new()
                            {
                                Label = $"Photo: {photo.Name}",
                                Address = $"Position : latitude {location.Latitude}, longitude {location.Longitude}",
                                Type = PinType.SavedPin,
                                Location = location,
                            },
                            IsAdded = true
                        };
                        
                        // -- envoie un message pour ajouter un pin sur la map
                        WeakReferenceMessenger.Default.Send(new PinMapMessage(pinMessage));
                        // -- ajoute la localisation dans la liste pour l'enregistrement
                        _localisations.Add(new LocalisationPhoto()
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            OrderIndex = _localisations.Count,
                            Timestamp = location.Timestamp,
                            Altitude = location.Altitude,
                            Accuracy = location.Accuracy,
                            Course = location.Course,
                            Speed = location.Speed,
                            VerticalAccuracy = location.VerticalAccuracy,
                            Name = photo.Name,
                            PathPhoto = photo.PathPhoto,
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur de la prise de photo";
                // gestion d'erreur simple (adapter selon besoins)
                System.Diagnostics.Trace.TraceError("Erreur lors de la prise de photo \n" + ex.Message);
            }
            finally
            {
                _takePhoto = false;
                if (IsMessageError)
                {
                    var cancellationToken = new System.Threading.CancellationToken();
                    // -- systeme de messagerie 
                    await Toast.Make($"{Message}",ToastDuration.Long).Show(cancellationToken);
                }
            }

        }

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
                { // -- stop pause
                    // -- mise a jour de temps de pause en seconde
                    var timePause = (DateTimeOffset.Now - _startPauseTime).TotalSeconds;
                    _routeTelemetry.TotalTimePaused += timePause;

                    IsPause = false;
                    PlayPause = "\ue1a2";
                    intent.SetAction(GeoAndroidService.ActionStopPause);
                    Android.App.Application.Context.StartService(intent);
                }
                else if (!IsPause && IsStart) // -- start pause
                {
                    _startPauseTime = DateTimeOffset.Now;
                    IsPause = true;
                    PlayPause = "\ue1c4";
                    intent.SetAction(GeoAndroidService.ActionPause);
                    Android.App.Application.Context.StartService(intent);
                }

                if (!IsStart)
                {
                    _routeTelemetry.DateTimeBegin = DateTimeOffset.Now;
                    IsEnablePhoto = true; // -- donne la possibilité de prendre des photos
                    IsStart = true;
                    PlayPause = "\ue1a2";

                    // -- démarrage du service android pour marcher en arrière plan
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
                    _serviceGeo.Pause = true;
                    PlayPause = "\ue1a2";
                }
                else
                {
                    _serviceGeo.Pause = false;
                    PlayPause = "\ue1c4";
                    IsEnablePhoto = true; // -- donne la possibilité de prendre des photos
                }

                _serviceGeo.StartLocationUpdatesAsync();
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
                // -- affichage du message erreur
                if ( IsMessageError)
                {
                    IsStart = false;
                    IsPause = false;
                    IsEnablePhoto = false; // -- désactive la prise de photo
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
            var popupAttente = new LoadingPage();
            
            try
            {
                if (_localisations.Count > 0)
                {
                    var popup = new PopupNameLocalisationGroup();
                    var name = await _dialogService.ShowPopupAsync<string>(popup, new PopupOptions
                    {
                        CanBeDismissedByTappingOutsideOfPopup = false,
                        Shape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(20, 20, 20, 20),
                            StrokeThickness = 2,
                            Stroke = Colors.LightGray
                        }
                    }, new CancellationToken());

                    if (name is null) return;

                    // -- création d'un service pour marcher en arrière plan
                    // -- pour arrêter le service
                    StopService();

                    RegisterDTO? register = await _serviceSaveUser.GetRegister();

                    if (register is null || string.IsNullOrEmpty(register.Id))
                    {
                        IsMessageError = true;
                        Message = "L'utilisateur n'est pas connecté.";
                        return;
                    }

                    _routeTelemetry.DateTimeEnd = DateTimeOffset.Now;
                    var localisationGroup = new LocalisationGroup()
                    {
                        //Name = $"Localisation_{DateTime.Now:yyyyMMdd_HHmmss}",
                        Name = name ?? $"Localisation_{DateTime.Now:yyyyMMdd_HHmmss}",
                        Date = DateTimeOffset.Now,
                        ApplicationUserId = register.Id, // -- à adapter selon l'authentification
                        Localisations = _localisations,
                        RouteTelemetry = _routeTelemetry,
                    };
                    
                    _dialogService.ShowPopup(popupAttente);
                    if ( await _saveLocalisation.SaveLocalisation(localisationGroup, new System.Threading.CancellationToken()))
                    {
                        //MesPhotos.Clear();
                        SheetViewModel.DeleteAllPhotos();

                        // -- enregistrement ok
                        _localisations.Clear();
                        // -- initalisation de la map au niveau de Polyne
                        WeakReferenceMessenger.Default.Send(new PolyneMapMessage(null));
                        // -- initialisation des pins sur la map
                        WeakReferenceMessenger.Default.Send(new PinMapMessage(null));
                    }
                    await _dialogService.ClosePopup(popupAttente);
                }
                // -- initialisation de la map sur la position de l'utilisateur
                await Task.Run(async () => await GetUserLocationAsync());

                IsEnablePhoto = false; // -- désactive la prise de photo
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
                    await _dialogService.ClosePopup(popupAttente);
                    var cancellationToken = new System.Threading.CancellationToken();
                    await Toast.Make($"{Message}",ToastDuration.Long).Show(cancellationToken);
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
            var localition = await _serviceGeo.GetCurrentLocationAsync(new CancellationTokenSource().Token);
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
                    //_distance += _lastLocation.CalculateDistance(location, DistanceUnits.Kilometers);
                    var route = _routeTelemetry;
                    _serviceTelemetry.CalculateTelemetry(_lastLocation, localisation, ref route);
                    _routeTelemetry = route;

                    SheetViewModel.GetRouteTeletry(_routeTelemetry);
                }
                _lastLocation = localisation; // -- pour la mise a jour du dernier point pour le calcul de la distance
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

        /// <summary>
        /// méthod qui arrête la prise des points Gps
        /// </summary>
        private void StopService()
        {
            if (IsStart)    // -- si le service est démarré on peut l'arrêter 
            {
#if ANDROID
                // -- création d'un service pour marcher en arrière plan
                var intent = new Intent(Android.App.Application.Context, typeof(GeoAndroidService));
                intent.SetAction(GeoAndroidService.ActionStop);
                Android.App.Application.Context.StartService(intent);
#endif

#if !ANDROID
            _serviceGeo.StopLocationUpdates();    
#endif
            }
        }
        #endregion

        #region public method eventHandler
        /// <summary>
        /// évènement pour 
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public void OnLocalication_Changed(object? sender, GeolocationLocationChangedEventArgs e)
        {
            TraitementLocalisation(e.Location);
        }


        public void SaveLocalisation (object? send, EventArgs args)
        {
            // -- pendant la prise photo ne pas faire de backup
            if (_takePhoto) return;

            // -- si le fichier existe le back up est déjà fait
            if (_serviceBackupGps.FileExist()) return; 

            // le service a démarré & il y a des points de localisation
            if (IsStart && _localisations.Count > 0)
            { // -- sauvegarde
                _serviceBackupGps.SavePointsLocalisation(_localisations);
            }

        }

        /// <summary>
        /// method lancer à la fermeture de l'application
        /// </summary>
        /// <param name="send"></param>
        /// <param name="args"></param>
        public void DestroyingSaveLocalisation (object? send, EventArgs args)
        {
            if (_takePhoto) return;
            SaveLocalisation(send, args);
            StopService(); // -- on arrête le service propremement
        }

        /// <summary>
        /// method qui est lancer lors de la supression d'une photo
        /// elle va mettre à jour la liste de localisation
        /// et du pin sur la map
        /// </summary>
        /// <param name="send"></param>
        /// <param name="args"></param>
        public void DeletePhoto(object? send, PhotoEventArgs args)
        {
            var photo = args.PhotoDTO;

            // -- recherche de l'object LocalisationPhoto
            // -- dans la liste des localisations pour le supprimer
            var local = _localisations.OfType<LocalisationPhoto>()
                .FirstOrDefault(x => x.Name == photo.Name);

            if (local is not null)
            {
                PinMessage pinMessage = new()
                {
                    Pin = new()
                    {
                        Label = $"Photo: {photo.Name}",
                        Address = $"Position : latitude {local.Latitude}, longitude {local.Longitude}",
                        Type = PinType.Place,
                        Location = local.GetLocation()
                    },
                    IsAdded = false
                };
                // -- envoi vers le behavior pour supprimer le pin
                WeakReferenceMessenger.Default.Send(new PinMapMessage(pinMessage));

                _localisations.Remove(local);
            }
        }
        #endregion
    }
}
