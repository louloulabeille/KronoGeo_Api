using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class MessageAuthentification(UserManager<IdentityUser> userManager
        , IdentityUser user) : IMessageMail
    {

        #region private properties
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        #endregion

        #region public method Factory
        public string ReturnMessage()
        {
            StringBuilder message = new();
            message.AppendLine($"<h1>Bonjour {_user.UserName} </h1>");
            message.AppendLine("<p>Merci de vous êtes inscrit sur notre plateforme KronoGeo. Veuillez cliquer sur le lien suivant pour confirmer votre adresse email : </p>");
            message.AppendLine($"<a href='{GenerationUrlAuthentification()}' target='_blank'>Confirmer mon adresse email</a>"); 
            message.AppendLine("<p>Attention : ce lien expire dans 24 heures.</p>");
            message.AppendLine($"<p>Si vous n'avez pas créé de compte, veuillez ignorer cet email.</p>");
            message.AppendLine("<p>Cordialement,</p>");
            return message.ToString();  
        }
        #endregion

        #region private method
        /// <summary>
        /// générateur url pour l'authentification du mail 
        /// </summary>
        /// <returns></returns>
        private string GenerationUrlAuthentification()
        {
            var token = _userManager.GenerateEmailConfirmationTokenAsync(_user).Result;
            //var url = new Uri($"https://localhost:7291/api/v1/Authenticate/ConfirmEmail/{Uri.EscapeDataString(_user.Id)}/{Uri.EscapeDataString(token)}");
            var url = new Uri($"https://localhost:7291/api/v1/Authenticate/ConfirmEmail/{Uri.EscapeDataString(_user.Id)}/{token}");
            return url.ToString();
        }
        #endregion
    }
}
