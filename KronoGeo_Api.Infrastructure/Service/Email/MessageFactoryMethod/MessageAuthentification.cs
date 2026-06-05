using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class MessageAuthentification(UserManager<IdentityUser> userManager
        , IdentityUser user, IOptions<UrlOptions> urlOptions) : IMessageMail
    {

        #region private properties
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        #endregion

        #region public method Factory
        public string ReturnMessage()
        {
            string lien = GenerationUrlAuthentification();

            StringBuilder message = new();
            message.AppendLine($"<h1>Bonjour {_user.UserName} </h1>");
            message.AppendLine("<p>Merci de vous êtes inscrit sur notre plateforme KronoGeo. Veuillez cliquer sur le lien suivant pour confirmer votre adresse email : </p>");
            message.AppendLine($"<a href='{lien}' target='_blank'>Confirmer mon adresse email</a>"); 
            message.AppendLine("<p>Attention : ce lien expire dans 72 heures.</p>");
            message.AppendLine($"<p>Si vous n'avez pas créé de compte, veuillez ignorer cet email.</p>");
            message.AppendLine("<p>Cordialement,</p>");
            message.AppendLine("");
            message.AppendLine($"ps:");
            message.AppendLine($"Si vous avez un problème avec le lien de confirmation, vous pouvez copier ce lien dans votre navigateur.");
            message.AppendLine($"{lien}");
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
            string encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
            string encodedId = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(_user.Id));

            //var url = new Uri($"https://localhost:7291/api/v1/Authenticate/ConfirmEmail/{encodedId}/{encodedToken}");
            var url = new Uri($"{_urlOptions.Value.UrlEmailAuthentification}/{encodedId}/{encodedToken}");
            return url.ToString();
        }
        #endregion
    }
}
