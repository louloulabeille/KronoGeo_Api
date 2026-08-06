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
        #endregion

        #region method partial OnChanged - après changement des properties sauvegarde en mémoire
        partial void OnIsBiometricChanged(bool value)
        {
            _saveParametrage.SaveParam(nameof(IsBiometric), value);
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
        }

        #endregion

    }
}
