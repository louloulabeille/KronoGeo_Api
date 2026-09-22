using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JetBrains.Annotations;
using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using System.Xml.Linq;

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
        public partial bool IsMiles { get; set; } = false;
        [ObservableProperty]
        public partial bool IsLocationManager { get; set; } = true;
        [ObservableProperty]
        public partial bool IsEnableLocationManager { get; set; } = false;
        #endregion

        #region public properties
        public ObservableCollection<View> LayoutDynamiques { get; } = [];
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

        async partial void OnIsLocationManagerChanged (bool value)
        {
            _saveParametrage.SaveParam(nameof(IsLocationManager), value);

            // -- création du message
            string text = "Pour que le changement se fasse, il faut relancer le système de géolocalisation.";
            ToastDuration duration = ToastDuration.Long;
            double fontSize = 14;

            // -- affichage du message
            var toast = Toast.Make(text, duration, fontSize);
            var cancellationToken = new System.Threading.CancellationToken();
            await toast.Show(cancellationToken);
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
            IsLocationManager = (bool)_saveParametrage.GetParam(nameof(IsLocationManager), true);
            InitElementsParam();
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


        #region private method 
        /// <summary>
        ///  Affiche le switch pour choisir le système de navigation possible
        ///  entre Fused et Location manager
        /// </summary>
        private void InitElementsParam()
        {
#if ANDROID26_0_OR_GREATER
            IsEnableLocationManager = true;

#endif
        }
        #endregion

    }
}
