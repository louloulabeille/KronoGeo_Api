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
        public partial bool IsMessageErreur { get; set; } = false;
        [ObservableProperty]
        public partial string Label_MessageErreur { get; set; } = string.Empty;
        #endregion

        #region public ObservableProperty properties saisie
        [ObservableProperty]
        public partial string Login { get; set; } = string.Empty;
        [ObservableProperty]
        public partial string Password { get; set; } = string.Empty;
        #endregion



        #region public method RelayCommand
        /// <summary>
        /// method pour afficher le mot de passe
        /// </summary>
        [RelayCommand]
        public void IsVisibilityPassword()
        {
            IsPassword = !IsPassword;
            Btn_IsPasswordTxt = IsPassword ? MaterialDesignIconsFonts.Visibility : MaterialDesignIconsFonts.Visibility_off;
        }

        /// <summary>
        /// method de connexion et gestion des messages de retour
        /// </summary>
        /// <returns></returns>
        [RelayCommand]
        public async Task GetLogin()
        {
            IsMessageErreur = false;
            Label_MessageErreur = string.Empty;
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
                        switch (result.Message)
                        {
                            case "Invalid login or password.":
                                Label_MessageErreur = "Mot de passe ou identifiant erroné";
                                break;
                            case "Your account is locked. Please try again later.":
                                Label_MessageErreur = "Votre compte est bloqué. Veuillez re-essayer plus tard.";
                                break;
                            default:
                                Label_MessageErreur = "Erreur interne. Veuillez re-essayer plus tard ou contactez administrateur.";
                                break;
                        }
                        IsMessageErreur = true;
                    }
                }catch(Exception ex)
                { // - mettre en place d'un systeme pour récupérer les messages d'erreurs ou pas
                    Console.WriteLine(ex.Message);
                }   
            }
        }

        #endregion
    }
}
