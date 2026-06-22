using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Api.Models;
using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.ModelViews
{
    public partial class ApplicationPageViewModel : ObservableObject
    {
        #region private readonly properties
        private readonly IServiceGeolocalisation _service;
        // -- mettre le service d'enregistrement
        private readonly List<Location> _locations;
        private readonly List<Localisation> _localisations;
        #endregion

        #region constructeur
        public ApplicationPageViewModel(IServiceGeolocalisation service)
        {
            _service = service;
            _locations = [];
            _localisations = [];

            _service.LocationChanged += OnLocalication_Changed;
        }
        #endregion

        #region public propeties ObservableProperties
        [ObservableProperty]
        public partial string Message { get; set; } = string.Empty;
        [ObservableProperty]
        public partial bool IsMessageError { get; set; } = false;
        #endregion


        #region method RelayCommand
        [RelayCommand]
        public static async Task ToolbarItem()
        {
            await Shell.Current.GoToAsync("ParametragePage");
        }

        [RelayCommand]
        // -- déclanché après l'affichage de la page Appearing
        public static async Task AppearingExe(BindableObject bind)
        {
            //Shell.SetTabBarIsVisible(bind, true);
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
            _locations.Add(e.Location);
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
