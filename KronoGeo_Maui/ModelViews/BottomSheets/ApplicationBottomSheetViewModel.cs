using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Carousel;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Model.Maui;
using KronoGeo_Api.Models.ModelEventArgs;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.Applications.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using The49.Maui.BottomSheet;

namespace KronoGeo_Maui.ModelViews.BottomSheets
{
    public partial class ApplicationBottomSheetViewModel : ObservableObject, IDisposable
    {
        #region public event handler
        /// <summary>
        /// Event qui met a jour quand une photo est supprimer
        /// </summary>
        public EventHandler<PhotoEventArgs>? DeletePhoto { get;set; }
        #endregion

        #region public properties
        public ObservableCollection<PageBaseViewModel>? MesPages { get; set; } = [];
        #endregion

        #region private readonly properties service
        private readonly IServiceCamera _camera;
        private readonly IServiceDatabaseMaui _dbMaui;
        #endregion

        #region private properties
        private RouteTelemetryMaui? _routeTelemetry = default;
        private bool? _isListGroupTrace;
        // -- tracé initial avant recherche
        private List<LocalisationGroupTraceMaui>? _listGroupTrace = default;
        #endregion

        #region public observable properties
        [ObservableProperty]
        public partial ObservableCollection<LocalisationGroupTraceMaui> MesTraces { get; set; } = [];
        [ObservableProperty]
        public partial ObservableCollection<PhotoDTO> MesPhotos { get; set; } = [];
        [ObservableProperty]
        public partial string MessageDistance { get; set; } = "0";
        [ObservableProperty]
        public partial string MessageVitesseMoyen { get; set; } = "0";
        [ObservableProperty]
        public partial string MessageNegativeElevation { get; set; } = "0";
        [ObservableProperty]
        public partial string MessagePostiveElevation { get; set; } = "0";
        [ObservableProperty]
        public partial string DateTimeBegin { get; set; }
        [ObservableProperty]
        public partial string SearchText { get; set; } = string.Empty;
        #endregion

        #region public constructeur
        public ApplicationBottomSheetViewModel(IServiceCamera camera, IServiceDatabaseMaui dbMaui)
        {
            // -- pour affichage des différentes pages du carousel
            /*MesPages = [];
            if (_isListGroupTrace)
            {
                MesPages.Add(new ListTraceViewModel());
            }
            else
            {
                MesPages.Add(new ListImageViewModel());
                MesPages.Add(new ResumeViewModel());
            }*/

            // -- chargement des services
            _camera = camera;
            _dbMaui = dbMaui;
            //MesPhotos = _camera.

            // -- chargement la date de début de la route
            DateTimeBegin = _routeTelemetry?.DateTimeBegin.ToString("dd/MM/yyyy HH:mm:ss")
                ?? string.Empty;
        }
        #endregion

        #region public method OnChanged
        /// <summary>
        /// Method qui filtre les tracés
        /// </summary>
        /// <param name="value"></param>
        public void OnSearchTextChanged(string value)
        {
            if (string.IsNullOrEmpty(value)) return;
            
            var traceFiltre = _listGroupTrace?.Where(t => t.Name.Contains(value, StringComparison.OrdinalIgnoreCase)).ToList();
            MesTraces.Clear();
            LoadTrace(traceFiltre ?? []);
        }
        #endregion

        #region public method
        /// <summary>
        /// Ajoute une photo dans le caroussel d'affichage des photos
        /// ListImageViewModel
        /// </summary>
        /// <param name="photo"></param>
        public void MiseAjourPhoto(PhotoDTO photo)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                MesPhotos.Add(photo);
            });
        }

        /// <summary>
        /// supprime les photos dans la liste du BottomSheet
        /// </summary>
        public void ClearAllPhotos()
        {
            MesPhotos.Clear();
        }

        /// <summary>
        /// Supprimes toutes les photos dans l'applications
        /// </summary>
        public void DeleteAllPhotos()
        {
            MainThread.BeginInvokeOnMainThread(() => {
                MesPhotos.Clear();
                _camera.DeletePhotos();
            });
        }

        /// <summary>
        /// method de mise a jour de la telemtry pour le BottomSheet
        /// </summary>
        /// <param name="route"></param>
        public void GetRouteTeletry ( RouteTelemetryMaui route )
        {
            _routeTelemetry = route;
            string unit = "km";
            string elevation = "m";
            if ( route.DistanceUnit == DistanceUnits.Miles )
            {
                unit = "mi";
                elevation = "ft";
            }
            DateTimeBegin = _routeTelemetry.DateTimeBegin.ToString("dd/MM/yyyy HH:mm:ss");
            MessageDistance = $"{Math.Round(_routeTelemetry.Distance, 3)} {unit}";
            MessageVitesseMoyen = $"{Math.Round(_routeTelemetry.AverageSpeed, 3)} {unit}/h";
            MessageNegativeElevation = $"{Math.Round(_routeTelemetry.NegativeElevationGain)} {elevation}";
            MessagePostiveElevation = $"{Math.Round(_routeTelemetry.PositiveElevationGain)} {elevation}";
        } 

        /// <summary>
        /// Pour l'affichage de la liste group tracé
        /// ou non sinon affichage des images et de la route Telemetry
        /// </summary>
        /// <param name="isListGroupTrace"></param>
        public async Task GetAfficheListGroup(bool isListGroupTrace)
        {
            if (_isListGroupTrace == isListGroupTrace) return;

            _isListGroupTrace = isListGroupTrace;

            var pages = new ObservableCollection<PageBaseViewModel>();
            if (isListGroupTrace)
            {
                pages.Add(new ListTraceViewModel());
            }
            else
            {
                pages.Add(new ListImageViewModel());
                pages.Add(new ResumeViewModel());
            }

            MesPages = null;
            MesPages = pages;      // le binding reçoit un PropertyChanged
            await Task.Yield();

            if (isListGroupTrace)
                await InitListeGroupTraceAsync();
        }
        #endregion

        #region private method
        /// <summary>
        /// Method qui remonte la liste des tracés
        /// </summary>
        /// <returns></returns>
        private async Task InitListeGroupTraceAsync()
        {
            var traces =  await _dbMaui.GetGroupsTraceAsync();
            _listGroupTrace = traces.OrderByDescending(t => t.Date).ToList();
            LoadTrace(_listGroupTrace);
        }

        /// <summary>
        /// Charge le tracés dans la liste des tracés
        /// </summary>
        /// <param name="traces"></param>
        private void LoadTrace( List<LocalisationGroupTraceMaui> traces )
        {
            foreach (var item in traces)
            {
                MesTraces.Add(item);
            }
        }
        #endregion

        #region public method RelayCommand
        /// <summary>
        /// method de suppression d'une photo en mémoire
        /// et de la localisation associée
        /// </summary>
        /// <param name="photo"></param>
        [RelayCommand]
        public void DeleteImage(PhotoDTO photo)
        {
            if (string.IsNullOrEmpty(photo.Name)) return;

            if (_camera.DeletePhoto(photo.PathComplet ?? string.Empty))
            {
                MesPhotos.Remove(photo);
                
                // -- code de l'eventHandler à faire au niveau de la fenêtre d'appel
                DeletePhoto?.Invoke(this, new(photo));

            }
        }

        /// <summary>
        /// Ferme le Bottom Sheet
        /// </summary>
        /// <param name="sheet"></param>
        /// <returns></returns>
        [RelayCommand]
        public static async Task CloseBottomSheet (BottomSheet? sheet)
        {
            if (sheet is null) return;
            await sheet.DismissAsync();
        }

        /// <summary>
        /// Charge le tracé au niveau de la Map
        /// </summary>
        /// <param name="localisationGroup"></param>
        public static void LoadTrace(LocalisationGroupTraceMaui localisationGroup)
        {
            if (localisationGroup is null) return;
            // -- envoie du message pour chargement de la trace
            //WeakReferenceMessenger.Default.Send(new LoadTraceMessage(localisationGroup));
        }

        #endregion


        #region interface method IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
