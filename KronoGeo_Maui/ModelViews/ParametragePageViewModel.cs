using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.ModelViews
{
    public partial class ParametragePageViewModel (IServiceSaveParametrage saveParametrage) : ObservableObject
    {
        #region private properties
        private readonly IServiceSaveParametrage _saveParametrage = saveParametrage;
        #endregion

        #region public properties ObservableProperty
        [ObservableProperty]
        public partial bool IsBiometric { get; set; } = false;
        [ObservableProperty]
        public partial bool IsMetric { get; set; } = true;
        [ObservableProperty]
        public partial bool IsMiles  { get; set; } = false;
        #endregion

        #region method partial OnChanged - après changement des properties sauvegarde en mémoire 
        partial void OnIsBiometricChanged(bool value)
        {
            _saveParametrage.SaveParam(nameof(IsBiometric), value);
        }

        partial void OnIsMetricChanged (bool value)
        {
            _saveParametrage.SaveParam(nameof(IsMetric), value);
            IsMiles = !value;
        }

        partial void OnIsMilesChanged (bool value)
        {
            _saveParametrage.SaveParam(nameof(IsMiles), value);
            IsMetric = !value;
        }
        #endregion

        #region public method RelayCommand
        /// <summary>
        /// methode d"initalisation de la fenêtre
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task AppearingExe()
        {
            IsBiometric = (bool)_saveParametrage.GetParam(nameof(IsBiometric), false);
            IsMetric = (bool)_saveParametrage.GetParam(nameof(IsMetric), true);
            IsMiles = (bool)_saveParametrage.GetParam(nameof(IsMiles), false);
        }

        #endregion

        #region public method

        /// <summary>
        /// method d'appel du bouton retour pour revenir à la page d'application
        /// </summary>
        public static void BackButtonPressed()
        {
            Shell.Current.GoToAsync("ApplicationPage");
        }
        #endregion


    }
}
