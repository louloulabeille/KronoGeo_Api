using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.ModelViews
{
    public partial class ApplicationPageViewModel : ObservableObject
    {

        #region constructeur
        public ApplicationPageViewModel()
        {
            

           /* // S'abonne au clic de l'onglet
            WeakReferenceMessenger.Default.Register<ActionStartGeo>(this, (r, m) =>
            {
                if ( m.ActionStart ) // -- la geolocation est démarrée
                {

                }
                else    // -- la geolocation n'a pas démarrée 
                { 

                }
            });*/
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
    }
}
