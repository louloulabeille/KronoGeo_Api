using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.ModelViews
{
    public partial class ApplicationPageViewModel : ObservableObject
    {
        [RelayCommand]
        public static async Task ToolbarItem()
        {
            await Shell.Current.GoToAsync("ParametragePage");
        }

        [RelayCommand]
        public static async Task AppearingExe(BindableObject bind)
        {
            Shell.SetTabBarIsVisible(bind, true);
        }
    }
}
