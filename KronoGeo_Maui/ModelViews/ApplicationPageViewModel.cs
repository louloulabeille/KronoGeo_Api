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
        //private readonly List<Location> _locations;
        private readonly List<Localisation> _localisations;
        #endregion

        #region constructeur
        public ApplicationPageViewModel(IServiceGeolocalisation service)
        {
            _service = service;
            //_locations = [];
            _localisations = [];

            _service.LocationChanged += OnLocalication_Changed;
        }
        #endregion

        #region public propeties ObservableProperties
        [ObservableProperty]
        public partial string Message { get; set; } = string.Empty;
        [ObservableProperty]
        public partial bool IsMessageError { get; set; } = false;
        [ObservableProperty]
        public partial string PlayPause { get; set; } = "\ue1c4"; // - affichage de play
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

        [RelayCommand]
        public void StartPause()
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
        }

        [RelayCommand]
        public void Stop()
        {
            try
            {
                _service.StopLocationUpdates();
            }
            catch(Exception ex)
            {
                IsMessageError = true;
                Message = "Erreur interne";
                Console.WriteLine(ex.Message);
            }
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
            Message = e.Location.ToString();
            IsMessageError = true;
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
