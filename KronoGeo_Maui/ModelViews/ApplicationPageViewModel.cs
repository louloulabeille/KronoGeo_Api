#if ANDROID
using Android.Content;
using Android.Gms.Tasks;
using Android.OS;
using KronoGeo_Maui.Platforms.Android.Applicatif.Geolocalisation;
using Android.Provider;
using Xamarin.Google.Crypto.Tink.Signature;
#endif

using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using CancellationTokenSource = System.Threading.CancellationTokenSource;
using Task = System.Threading.Tasks.Task;
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
using KronoGeo_Maui.Applications.Factory.Geolocalisation;
using KronoGeo_Api.Models.Model.Maui;
using Application = Microsoft.Maui.Controls.Application;

#if ANDROID
using Android.Util;
using Android.App;
#endif
namespace KronoGeo_Maui.ModelViews
{
    enum MapTypeEnum
    {
        Street,
        Satellite,
        Hybrid
    }
    /// <summary>
    /// Supression de IRecipient pour le système de messenger pour faire une approche explicite au niveau de l'appel
    /// Création du methode Event qui est appelé lors de la déclartion du register du système Messenger
    /// </summary>
    public partial class ApplicationPageViewModel : ObservableObject, IDisposable ,IRecipient<LocationChangedMessage>
    {
        #region private readonly properties
        private readonly IServiceGeolocalisation _serviceGeo;
        //private readonly List<Localisation> _localisations;
        private readonly IServiceSaveLocalisation _saveLocalisation;
        private readonly IServiceCamera _camera;
        private readonly IDialogService _dialogService;
        private readonly IServiceSaveUser _serviceSaveUser;
        private readonly IServiceTelemetry _serviceTelemetry;
        //private readonly IServiceBackupGps _serviceBackupGps;
        private readonly IServiceProvider _serviceProvider;
        private readonly IServicePermissions _servicePermissions;
        private readonly IServiceDatabaseMaui _dbSqlite;

        #endregion
#if ANDROID
        private readonly IServiceSaveParametrage? _parametrage;
        private readonly IServiceBattery? _serviceBattery;
#endif

        #region private properties
        private LocalisationMaui? _lastLocation { get; set; } = default;
        private RouteTelemetryMaui _routeTelemetry { get; set; } = new RouteTelemetryMaui();
        //private Localisation? _lastLocation { get; set; } = default;
        //private RouteTelemetry _routeTelemetry { get; set; }
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
        public bool IsDesactiveBatteySaver { get; set; } = false;
        #endregion

        #region public propeties ObservableProperties
        /// <summary>
        /// pour savoir si le service est en pause ou non
        /// par rapport à l'affichage de l'icone play ou pause
        /// </summary>
        [ObservableProperty]
        public partial string PlayPause { get; set; } = "\ue1c4"; // - affichage de play 
        /// <summary>
        /// pour savoir si l'affichage de la map est en mode Street ou Satellite ou hybride
        /// </summary>
        [ObservableProperty]
        public partial string MapType { get; set; } = "Hybrid";
        /// <summary>
        /// Desactive la possibilité de prendre des photos 
        /// si le service de géolocalisation n'est pas démarré
        /// </summary>
        [ObservableProperty]
        public partial bool IsEnablePhoto { get; set; } = false;
        /// <summary>
        /// Affiche - active le bouton de sauvegarde des géolocalisations au niveau du serveur
        /// </summary>
        [ObservableProperty]
        public partial bool IsEnableSave { get; set; } = false;
        #endregion

        #region constructeur
        public ApplicationPageViewModel(FactoryGeolocation factoryGeolocation
            , IServiceSaveLocalisation saveLocalisation, IServiceCamera camera
            , IDialogService dialogService, IServiceSaveUser serviceSaveUser
            , IServiceTelemetry serviceTelemetry
            // , IServiceBackupGps serviceBackupGps
            , ApplicationBottomSheetViewModel sheetViewModel
            , IServiceProvider serviceProvider , IServicePermissions servicePermissions
            , IServiceBattery? serviceBattery, IServiceSaveParametrage? parametrage
            , IServiceDatabaseMaui sqliteMaui)
        {
            // -- pour affichage des différentes pages du carousel
            /*MesPages = [];
            //MesPages.Add(new MapViewModel());
            MesPages.Add(new ListImageViewModel());
            MesPages.Add(new ResumeViewModel());*/

            // -- chargement des services
            _serviceGeo = factoryGeolocation.GetServiceGeolocalisation();
            _camera = camera;
            _dialogService = dialogService;
            _serviceSaveUser = serviceSaveUser;
            _serviceTelemetry = serviceTelemetry;
            //_serviceBackupGps = serviceBackupGps;
            _serviceProvider = serviceProvider; //-- service pour l'injection de dépendance
            _servicePermissions = servicePermissions;
            _dbSqlite = sqliteMaui;

#if !ANDROID
            _serviceGeo.LocationChanged += async(sender, e) => await OnLocalication_Changed(sender, e);
#endif
#if ANDROID
            _serviceBattery = serviceBattery;
            _parametrage = parametrage;
#endif

            //_localisations = [];
            _saveLocalisation = saveLocalisation;
            // -- initialise l'object télémétrie
            //_routeTelemetry = _serviceTelemetry.CalculateTelemetry(_localisations);
            //_routeTelemetry = _serviceTelemetry.CalculateTelemetry([]);

            // -- chargement du view model ApplicationBottomSheetViewModel
            SheetViewModel = sheetViewModel;
            

            // -- enregistrer dans le registre des messages pour recevoir les messages de type LocationChangedMessage
            //WeakReferenceMessenger.Default.RegisterAll(this);
            //WeakReferenceMessenger.Default.Register<LocationChangedMessage>(this);

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
            var windowList = Application.Current?.Windows;
            var window = (windowList is not null && windowList.Count > 0) ? windowList[0] : null;
            if (window is not null)
            {
                window.Stopped += async (sender, e) => await SaveLocalisation(sender, e);
                window.Destroying += async(sender, e) => await DestroyingSaveLocalisation(sender, e);

                SheetViewModel.DeletePhoto += async (sender, e) => await DeletePhoto(sender, e);
            }
        }

        /// <summary>
        /// lancement après le chargement de la fenêtre - appelé qu'une seul fois
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task LoadedExe()
        {

            // -- mise a jour dans la Map de la geolocalisation sinon il affiche la map par défaut
            await GetUserLocationAsync();

            // - chargement du popup si un backup existe pour recharger les points dedans
            //if (_serviceBackupGps.FileExist() || await _dbSqlite.Count() > 0)
            if ( await _dbSqlite.Count() > 0)
            {
                var popup = new PopupRechargementPage();
                var result = await _dialogService.ShowPopupAsync<string>(popup, new PopupOptions
                {
                    CanBeDismissedByTappingOutsideOfPopup = false,
                }, new CancellationToken());

                // - traitement du résultat
                if ( result is not null && bool.Parse(result) )
                {   // -- recharge les données
                    //var localisations = _serviceBackupGps.ReturnLocalisation();
                    var localisations = await _dbSqlite.GetAllAsync();
                    if ( localisations is not null && localisations.Count > 0)
                    {
                        //_localisations.Clear();
                        //_localisations.AddRange(localisations.OrderBy(ob => ob.OrderIndex));
                        //_lastLocation = (_localisations.Count > 0) ? _localisations[_localisations.Count - 1] : null;
                        _lastLocation = (localisations.Count > 0) 
                            ? localisations.OrderBy(ob => ob.OrderIndex).ToList()[^1] : null;

                        //_routeTelemetry = _serviceTelemetry.CalculateTelemetry(localisations);

                        // -- passage de la RouteTelemetry vers le BottomSheet
                        SheetViewModel.GetRouteTeletry(_routeTelemetry);

                        // -- supprimer en mémoire les photos
                        SheetViewModel.ClearAllPhotos();

                        // -- mise a jour de la carte
                        foreach (var item in localisations) {
                            var location = item.GetLocation();
                            if (item.Name is not null)  // -- localisationMauiPhoto
                            {
                                // -- chargement des photos dans le carroussel correspondant 
                                // -- du BottomSheet
                                SheetViewModel.MiseAjourPhoto(new()
                                {
                                    Name = item.Name,
                                    PathPhoto = item.PathPhoto
                                });
                                // -- mise a jour du pîn de la map
                                PinMessage pinMessage = new()
                                {
                                    Pin = new()
                                    {
                                        Label = $"Photo: {item.Name}",
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
                        // _serviceBackupGps.DeleteFile();
                    }
                }
                else
                {
                    // _serviceBackupGps.DeleteFile();
                    await _dbSqlite.Clear(); // -- supprime toutes les données de la base de données
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
            var windowList = Application.Current?.Windows;
            var window = (windowList is not null && windowList.Count > 0) ? windowList[0] : null;
            if (window is not null)
            {
                window.Stopped -= async (sender, e) => await SaveLocalisation(sender, e); // -- quand l'application perd le focus ou passe en arrière plan
                window.Destroying -= async (sender, e) => await DestroyingSaveLocalisation(sender, e);
                SheetViewModel.DeletePhoto -= async (sender, e) => await DeletePhoto(sender, e);
            }
        }
        #endregion

        #region method RelayCommand
        /// <summary>
        /// Commande pour l'ouverture du ButtonSheet
        /// </summary>
        /// <returns></returns>
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
            
#if ANDROID
            Platform.CurrentActivity?.FinishAffinity();
            Java.Lang.JavaSystem.Exit(0);
#elif WINDOWS
            Microsoft.Maui.Controls.Application.Current?.Quit();
#elif IOS
            System.Environment.Exit(0);
#endif
        }

        /// <summary>
        /// method de prise de photo et enregistrement de la localisation
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task TakePhoto()
        {
            Message = string.Empty;
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
                        LocalisationMaui photoLocalisation = new()
                        {
                            Latitude = location.Latitude,
                            Longitude = location.Longitude,
                            //OrderIndex = _localisations.Count,
                            OrderIndex = (await _dbSqlite.Count()),
                            Timestamp = location.Timestamp,
                            Altitude = location.Altitude,
                            Accuracy = location.Accuracy,
                            Course = location.Course,
                            Speed = location.Speed,
                            VerticalAccuracy = location.VerticalAccuracy,
                            Name = photo.Name,
                            PathPhoto = photo.PathPhoto
                        };
                        // -- ajout de la localisation dans la base donnée locale
                        await _dbSqlite.SaveAsync(photoLocalisation);
                        // -- ajoute la localisation dans la liste pour l'enregistrement
                        //_localisations.Add(photoLocalisation);

                        // -- envoie un message pour mettre à jour le tracé sur la map
                        WeakReferenceMessenger.Default.Send(new PolyneMapMessage(location));
                    }
                }
            }
            catch (Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur de la prise de photo";
                // gestion d'erreur simple (adapter selon besoins)
                System.Diagnostics.Trace.TraceError("Erreur lors de la prise de photo \n" + ex.Message);

#if ANDROID
                Log.Error("GeoAndroidService", ex.Message);
#endif

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
            Message = string.Empty;
            IsMessageError = false;
            IsEnableSave = false;
            try
            {
#if ANDROID
                // -- demande de permission pour les notifications 
                if (!await _servicePermissions.GetNotificationPermissionAsync())
                {
                    IsMessageError = true;
                    Message = "Impossible de lancer le suivi sans la permission de notification au niveau de l'application.";
                }

                var intent = new Intent(Android.App.Application.Context, typeof(GeoAndroidService));
                
                if ( IsPause && IsStart )   // -- en pause et le service a démarré
                { // -- stop pause
                    // -- mise a jour de temps de pause en seconde
                    var timePause = (DateTimeOffset.Now - _startPauseTime).TotalSeconds;
                    _routeTelemetry.TotalTimePaused += timePause;

                    IsEnableSave = false;
                    IsEnablePhoto = true;
                    IsPause = false;
                    PlayPause = "\ue1a2";
                    intent.SetAction(GeoAndroidService.ActionStopPause);
                    Android.App.Application.Context.StartService(intent);
                }
                else if (!IsPause && IsStart) // -- start pause
                {
                    IsEnableSave = false;
                    IsEnablePhoto = false;
                    _startPauseTime = DateTimeOffset.Now;
                    IsPause = true;
                    PlayPause = "\ue1c4";
                    intent.SetAction(GeoAndroidService.ActionPause);
                    Android.App.Application.Context.StartService(intent);
                }

                if (!IsStart)
                {

                    // -- affichage du popup pour expliquer la problématique au niveau de la battery saver "économie d'énergie"
                    var afficheMessage = _parametrage is not null && (bool)_parametrage.GetParam("IsBatterySaver", true);
                    if (_serviceBattery is not null && _serviceBattery.IsBatterySaver() && afficheMessage)
                    {
                        var popup = new PopupBatterySaverPage();
                        var result = await _dialogService.ShowPopupAsync<string>(popup, new PopupOptions
                        {
                            CanBeDismissedByTappingOutsideOfPopup = false,
                        }, new CancellationToken());

                        // -- si true alors on ouvre le parameter 
                        if (result == "true")
                        {
                            IsDesactiveBatteySaver = true;
                            _serviceBattery?.OpenWindowBatterySaver();
                        }
                        
                    }

                    _routeTelemetry.DateTimeBegin = DateTimeOffset.Now; // -- type date heure local non utc
                    SheetViewModel.GetRouteTeletry(_routeTelemetry); // -- passage de la RouteTelemetry vers le BottomSheet 
                    IsEnablePhoto = true; // -- donne la possibilité de prendre des photos
                    IsStart = true;
                    PlayPause = "\ue1a2";

                    // -- abonnement vers la messagerie;
                    StartMessenger();

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

               await _serviceGeo.StartLocationUpdatesAsync();
#endif
            }
            catch (FeatureNotSupportedException fnsEx)
            {
                // Handle not supported on device exception
                IsMessageError = true;
                Message = "Votre matériel n'est pas supporté.";
                Console.WriteLine(fnsEx.Message);
#if ANDROID
                Log.Error("GeoAndroidService", fnsEx.Message);
#endif
            }
            catch (FeatureNotEnabledException fneEx)
            {
                IsMessageError = true;
                Message = "La géolocalisation n'est pas activée. Veuillez l'activer.";
                Console.WriteLine(fneEx.Message);
#if ANDROID
                Log.Error("GeoAndroidService", fneEx.Message);
#endif
            }
            catch (PermissionException pEx)
            {
                // Handle permission exception
                IsMessageError = true;
                Message = "La permission pour la géolocalisation ou notification n'ont pas été données.";
                Console.WriteLine(pEx.Message);
#if ANDROID
                Log.Error("GeoAndroidService", pEx.Message);
#endif

            }
            catch (Exception ex)
            {
                // Unable to get location
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
#if ANDROID
                Log.Error("GeoAndroidService", ex.Message);
#endif

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
                    await Toast.Make($"{Message}",ToastDuration.Long).Show(cancellationToken);
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
            Message = string.Empty;
            IsMessageError = false;
            try
            {
                //if (_localisations.Count > 0)
                if( await _dbSqlite.Count() > 0 )
                {
                    // -- mise a jour de la date de fin du parcours
                    _routeTelemetry.DateTimeEnd = DateTimeOffset.Now;

                    // -- création d'un service pour marcher en arrière plan
                    // -- pour arrêter le service
                    await StopService();

                    // -- arrêt sur l'écoute sur le systeme de message
                    WeakReferenceMessenger.Default.Unregister<LocationChangedMessage>(this);
                    
                    // -- ouverture d'une popup pour savoir si la personne veut sauvegarder ou pas
                    var popup = new PopupSauvegardePage();
                    var result = await _dialogService.ShowPopupAsync<string>(popup, new PopupOptions
                    {
                        CanBeDismissedByTappingOutsideOfPopup = false,
                        Shape = new RoundRectangle
                        {
                            CornerRadius = new CornerRadius(20, 20, 20, 20),
                            StrokeThickness = 2,
                            Stroke = Colors.LightGray
                        }
                    }, new CancellationToken());

                    if ( result is not null && result == "true" )
                    {
                        // -- sauvegarde
                        IsEnableSave = true;
                    }
                    else
                    {
                        // -- supression de toutes les données
                        IsEnableSave = false;
                        await InitWindow();
                    }

#if ANDROID
                    // -- affichage du popup pou expliquer la problématique au niveau de la battery saver "économie d'énergie"
                    var afficheMessage = _parametrage is not null && (bool)_parametrage.GetParam("IsBatterySaver", true);
                    if ( IsDesactiveBatteySaver && afficheMessage)
                    {
                        IsDesactiveBatteySaver = false;
                        _serviceBattery?.OpenWindowBatterySaver();
                    }
#endif

                }
                // -- initialisation de la map sur la position de l'utilisateur
                await Task.Run(async () => await GetUserLocationAsync());

                IsEnablePhoto = false;  // -- désactive la prise de photo
                PlayPause = "\ue1c4";   // -- changement de l'icone
                IsStart = false;
            }
            catch(Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
#if ANDROID
                Log.Error("GeoAndroidService", ex.Message);
#endif

            }
            finally
            {
                if (IsMessageError)
                {
                    //await _dialogService.ClosePopup(popupAttente);
                    var cancellationToken = new System.Threading.CancellationToken();
                    await Toast.Make($"{Message}",ToastDuration.Long).Show(cancellationToken);
                }
            }
        }

        /// <summary>
        /// method qui enregistre les points de localisation au niveau du serveur
        /// avec les photos
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task SaveAsync()
        {
            var popupAttente = new LoadingPage();
            Message = string.Empty;
            IsMessageError = false;

            try
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

                RegisterDTO? register = await _serviceSaveUser.GetRegister();

                if (register is null || string.IsNullOrEmpty(register.Id))
                {
                    IsMessageError = true;
                    Message = "L'utilisateur n'est pas connecté.";
                    return;
                }

                var localisationsMaui = await _dbSqlite.GetAllAsync();

                var localisationGroup = new LocalisationGroup()
                {
                    //Name = $"Localisation_{DateTime.Now:yyyyMMdd_HHmmss}",
                    Name = name ?? $"Localisation_{DateTime.Now:yyyyMMdd_HHmmss}",
                    Date = DateTimeOffset.Now,
                    ApplicationUserId = register.Id, // -- à adapter selon l'authentification
                    //Localisations = _localisations,
                    
                    Localisations = localisationsMaui.Select(l => l.ToLocalisation().Get()).ToList(),
                    RouteTelemetry = _routeTelemetry.ToLocalisation().Get(),
                };

                _dialogService.ShowPopup(popupAttente);
                if (await _saveLocalisation.SaveLocalisation(localisationGroup, new System.Threading.CancellationToken()))
                {
                    await InitWindow();
                }
                await _dialogService.ClosePopup(popupAttente);
                IsEnableSave = false;
                
            }
            catch (Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
#if ANDROID
                Log.Error("GeoAndroidService", ex.Message);
#endif

            }
            finally
            {
                if (IsMessageError)
                {
                    await _dialogService.ClosePopup(popupAttente);
                    var cancellationToken = new System.Threading.CancellationToken();
                    await Toast.Make($"{Message}", ToastDuration.Long).Show(cancellationToken);
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
            Message = string.Empty;
            IsMessageError = false;
            try
            {
                if (await _servicePermissions.GetLocalisationPermissionAsync())
                {
                    var localition = await _serviceGeo.GetCurrentLocationAsync(new CancellationTokenSource().Token);
                    if (localition is not null)
                    {
                        //MapRegion = MapSpan.FromCenterAndRadius(localition, Distance.FromMeters(500));
                        // -- envoie un message pour recentrer la map sur la position de l'utilisateur
                        WeakReferenceMessenger.Default.Send(new RecenterMapMessage(localition));
                    }
                }
                else
                {
                    IsMessageError = true;
                    Message = "Vous n'avez pas donné la permission d'utiliser la géolocalisation.";
                }
            } 
            catch( Exception ex )
            {
                IsMessageError = true;
                Message = "Gps désactivé, time out ou autres erreurs.";
                Console.WriteLine(ex.Message);
#if ANDROID
                Log.Error("GeoAndroidService",$"{ex.Message}");
#endif
            }
            finally
            {
                if ( IsMessageError )
                {
                    var cancellationToken = new System.Threading.CancellationToken();
                    await Toast.Make($"{Message}", ToastDuration.Long).Show(cancellationToken);
                }
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
        public async void Receive( LocationChangedMessage message )
        {
            try
            {
                await TraitementLocalisation(message.Value);
            }
            catch (Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
                var cancellationToken = new System.Threading.CancellationToken();
                await Toast.Make($"{Message}", ToastDuration.Long).Show(cancellationToken);
            }
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
        /// Pour gérer le lancement et relancement du système de géolocalisation 
        /// </summary>
        private void StartMessenger()
        {
#if ANDROID
            Log.Debug("GeoAndroidService", "********************************************************************");
            Log.Debug("GeoAndroidService", $"[{GetHashCode()}] StartMessenger appelé");
#endif
            // -- arrêt du register pour ne pas l'avoir en double
            WeakReferenceMessenger.Default.Unregister<LocationChangedMessage>(this);
            // -- register du messenger 
            WeakReferenceMessenger.Default.Register<LocationChangedMessage>(this);
            //WeakReferenceMessenger.Default.Register<LocationChangedMessage>(this, OnReceiveMessageLocation);
        }

        /// <summary>
        /// Method d'intialisation d'une fenêtre
        /// </summary>
        private async Task InitWindow()
        {
            _lastLocation = null;
            //MesPhotos.Clear();
            SheetViewModel.DeleteAllPhotos();
            Message = string.Empty;
            IsMessageError = false;

            // -- enregistrement ok
            //_localisations.Clear();
            await _dbSqlite.Clear(); // -- supprime toutes les données de la base de données
            // -- initalisation de la map au niveau de Polyne
            WeakReferenceMessenger.Default.Send(new PolyneMapMessage(null));
            // -- initialisation des pins sur la map
            WeakReferenceMessenger.Default.Send(new PinMapMessage(null));
            // -- reinitalise la telemetrie
            //_routeTelemetry = _serviceTelemetry.CalculateTelemetry(_localisations);
            var localisations = await _dbSqlite.GetAllAsync();
            _routeTelemetry = RouteTelemetryMaui.FromRouteTelemetry(_serviceTelemetry
                .CalculateTelemetry(localisations.Select(l => l.ToLocalisation().Get()).ToList())
                .GetDTO());
            // -- initialisation
            SheetViewModel.GetRouteTeletry(_routeTelemetry); // -- passage de la RouteTelemetry vers le BottomSheet
        }

        /// <summary>
        /// Method de traitement de la localisation
        /// </summary>
        /// <param name="location"></param>
        private async Task TraitementLocalisation (Location location)
        {
            LocalisationMaui localisation = new()
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
#if ANDROID
            Log.Debug("GeoAndroidService", $"Accuracy :{localisation.Accuracy} - Longitude : {localisation.Longitude} - Latitude : {localisation.Latitude}" );
#endif
            // -- si des doublons au niveau de eventchange Google lors de la mise a jour de la position, donc on ne garde que les nouvelles positions
            //if ( !_localisations.Exists(l=> l.Longitude == localisation.Longitude && l.Latitude == localisation.Latitude) )
            if (!await _dbSqlite.Exists(localisation))
                {
                if (_lastLocation is not null)
                {
                    // -- calcul de la distance entre la dernière position et la nouvelle
                    var route = _routeTelemetry.ToLocalisation().Get();
                    _serviceTelemetry.CalculateTelemetry(_lastLocation.ToLocalisation().Get(),
                        localisation.ToLocalisation().Get(), ref route);
                    _routeTelemetry = RouteTelemetryMaui.FromRouteTelemetry(route.GetDTO());
                    // -- faire le traitement dans le thread principal pour éviter les erreurs de cross thread
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        SheetViewModel.GetRouteTeletry(_routeTelemetry);
                    });
                }
                _lastLocation = localisation; // -- pour la mise a jour du dernier point pour le calcul de la distance

                // -- envoie un message pour recentrer la map sur la position de l'utilisateur
                //WeakReferenceMessenger.Default.Send(new RecenterMapMessage(location));

                // -- ajoute la localisation dans la liste pour l'enregistrement
                //int index = _localisations.Count;
                int index = await _dbSqlite.Count();
                localisation.OrderIndex = index; // -- mise a jour de l'index
                //_localisations.Add(localisation);
                await _dbSqlite.SaveAsync(localisation); // -- ajout de la localisation dans la base donnée locale
#if ANDROID
                Log.Debug("GeoAndroidService", $"Ajout localisation - Longitude : {localisation.Longitude}");
#endif
                // -- faire le traitement dans le thread principal pour éviter les erreurs de cross thread
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    // -- envoie un message pour mettre à jour le tracé sur la map
                    WeakReferenceMessenger.Default.Send(new PolyneMapMessage(location));
                });
            }
        }

        /// <summary>
        /// méthod qui arrête la prise des points Gps
        /// </summary>
        private async Task StopService()
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
            await _serviceGeo.StopLocationUpdatesAsync();    
#endif
            }
        }

        #endregion

        #region public method eventHandler
        /// <summary>
        /// évènement pour le traitement des locations pour windows et IOS
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        public async Task OnLocalication_Changed(object? sender, GeolocationLocationChangedEventArgs e)
        {
            await TraitementLocalisation(e.Location);
        }

        
        /// <summary>
        /// method pour la sauvegarde lors de la fermeture de l'application quand le service est encore lancé
        /// </summary>
        /// <param name="send"></param>
        /// <param name="args"></param>
        public async Task SaveLocalisation (object? send, EventArgs args)
        {
            // -- pendant la prise photo ne pas faire de backup
            if (_takePhoto) return;

            // -- si le fichier existe le back up est déjà fait
            //if (_serviceBackupGps.FileExist()) return; 

            var localisations = await _dbSqlite.GetAllAsync();
            // le service a démarré & il y a des points de localisation
            if (IsStart && localisations.Count > 0)
            { // -- sauvegarde
                //_serviceBackupGps.SavePointsLocalisation(_localisations);
                await _dbSqlite.SaveTelemetryAsync(_routeTelemetry); // -- sauvegarde de la télémétrie)
            }
        }

        /// <summary>
        /// method lancer à la fermeture de l'application
        /// </summary>
        /// <param name="send"></param>
        /// <param name="args"></param>
        public async Task DestroyingSaveLocalisation (object? send, EventArgs args)
        {
            if (_takePhoto) return;
            await SaveLocalisation(send, args);
            if (IsStart)
            {
                await StopService(); // -- on arrête le service propremement
                IsStart = false;
            }

        }

        /// <summary>
        /// method qui est lancer lors de la supression d'une photo
        /// elle va mettre à jour la liste de localisation
        /// et du pin sur la map
        /// </summary>
        /// <param name="send"></param>
        /// <param name="args"></param>
        public async Task DeletePhoto(object? send, PhotoEventArgs args)
        {
            var photo = args.PhotoDTO;

            // -- recherche de l'object LocalisationPhoto
            // -- dans la liste des localisations pour le supprimer
            LocalisationMaui? local = null;
            //for (int i = 0; i < _localisations.Count; i++)
            var localisations = await _dbSqlite.GetAllAsync();

            var localPhoto = localisations.FirstOrDefault(p => p.Name == args.PhotoDTO.Name);

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

                await _dbSqlite.DeleteAsync(local); // -- supprime la localisation de la base de données
                //_localisations.Remove(local);
            }
        }
        #endregion
    }
}
