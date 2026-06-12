using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Maui.Applications.Helpers;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.ModelViews
{
    public partial class MainPageViewModel(IServiceHttpKronoGeo http) : ObservableObject
    {
        #region private readonly properties
        private readonly IServiceHttpKronoGeo _http = http;
        #endregion

        #region public ObservableProperty properties
        [ObservableProperty]
        public partial bool IsPassword { get; set; } = true;
        [ObservableProperty]
        public partial string Btn_IsGestionGroup { get; set; } = MaterialDesignIconsFonts.Groups;
        [ObservableProperty]
        public partial string Btn_IsPasswordTxt { get; set; } = MaterialDesignIconsFonts.Visibility;
        [ObservableProperty]
        public partial string Login { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;
        #endregion


        #region public method RelayCommand

        [RelayCommand]
        public void IsVisibilityPassword()
        {
            IsPassword = !IsPassword;
            Btn_IsPasswordTxt = IsPassword ? MaterialDesignIconsFonts.Visibility : MaterialDesignIconsFonts.Visibility_off;
        }

        [RelayCommand]
        public async Task GetLogin()
        {
            if ( !string.IsNullOrEmpty(Login.Trim()) && !string.IsNullOrEmpty(Password.Trim()) )
            {
                var user = new RegisterDTO { Login = Login, Password = Password, };
                var result = await _http.AuthenticateAsync(user);
            }
            

        }

        #endregion
    }
}
