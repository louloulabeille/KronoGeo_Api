using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using KronoGeo_Api.Infrastructure.Service.Http;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Maui.Applications.Helpers;
using KronoGeo_Maui.Applications.Services;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace KronoGeo_Maui.ModelViews
{
    public partial class MainPageViewModel(IServiceHttpKronoGeo http, IServiceSaveUser saveUser) : ObservableObject
    {
        #region private readonly properties
        private readonly IServiceHttpKronoGeo _http = http;
        private readonly IServiceSaveUser _saveUser = saveUser;
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
                try
                {
                    var user = new RegisterDTO 
                    { 
                        Login = Login, Password = Password, 
                        Token = string.Empty, NewPassord = string.Empty 
                    };
                    var result = await _http.AuthenticateAsync(user);

                    // - enregistrement 
                    if(result.IsSuccess && result.Register is not null )
                    {
                        await _saveUser.SaveUser(result.Register);
                        var register = _saveUser.GetRegister();
                       /* var content = result.Content;
                        var jsonResponse = await result.Content.ReadAsStringAsync();
                        RegisterDTO? userResult = JsonSerializer.Deserialize<RegisterDTO>(jsonResponse, JsonOptions.GetJsonOptions());*/
                    }
                    else
                    { //  - affiche le message

                    }


                }catch(Exception ex)
                {
                    Console.WriteLine(ex.Message);
                }
                
            }
            

        }

        #endregion
    }
}
