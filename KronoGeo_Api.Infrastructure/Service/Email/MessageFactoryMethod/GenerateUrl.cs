using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class GenerateUrl
    {
        #region public method
        /// <summary>
        /// générateur url pour l'authentification du mail 
        /// </summary>
        /// <returns></returns>
        public static string GenerationUrlAuthentification(UserManager<IdentityUser> userManager
            , IOptions<UrlOptions> urlOptions, IdentityUser user)
        {
            var token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
            string encodedToken = EncodingMessage(token);
            string encodedId = EncodingMessage(user.Id);

            //var url = new Uri($"https://localhost:7291/api/v1/Authenticate/ConfirmEmail/{encodedId}/{encodedToken}");
            var url = new Uri($"{urlOptions.Value.UrlEmailAuthentification}/{encodedId}/{encodedToken}");
            return url.ToString();
        }

        /// <summary>
        /// method d'encodage pour echange des datas en web
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string EncodingMessage (string entry)
        {
            return WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(entry));
        }

        /// <summary>
        /// method de décodage pour récuper des datas web
        /// </summary>
        /// <param name="entry"></param>
        /// <returns></returns>
        public static string DecodingMessage (string entry)
        {
            return Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(entry));
        }

        #endregion
    }
}
