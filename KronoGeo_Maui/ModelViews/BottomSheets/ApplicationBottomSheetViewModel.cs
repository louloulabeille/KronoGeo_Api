using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Carousel;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.ModelEventArgs;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.Applications.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        public ObservableCollection<PageBaseViewModel> MesPages { get; set; }
        #endregion

        #region private readonly properties service
        private readonly IServiceCamera _camera;
        #endregion

        #region private method
        private RouteTelemetry? _routeTelemetry = default;
        #endregion

        #region public observable properties
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
        #endregion

        #region public constructeur
        public ApplicationBottomSheetViewModel(IServiceCamera camera)
        {
            // -- pour affichage des différentes pages du carousel
            MesPages = [];
            MesPages.Add(new ListImageViewModel());
            MesPages.Add(new ResumeViewModel());

            // -- chargement des services
            _camera = camera;
            //MesPhotos = _camera.
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
        public void GetRouteTeletry ( RouteTelemetry route )
        {
            _routeTelemetry = route;
            string unit = "km";
            string elevation = "m";
            if ( route.DistanceUnit == DistanceUnits.Miles )
            {
                unit = "mi";
                elevation = "ft";
            }
            MessageDistance = $"{Math.Round(_routeTelemetry.Distance, 3)} {unit}";
            MessageVitesseMoyen = $"{Math.Round(_routeTelemetry.AverageSpeed, 3)} {unit}/h";
            MessageNegativeElevation = $"{Math.Round(_routeTelemetry.NegativeElevationGain)} {elevation}";
            MessagePostiveElevation = $"{Math.Round(_routeTelemetry.PositiveElevationGain)} {elevation}";
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
        #endregion


        #region interface method IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
