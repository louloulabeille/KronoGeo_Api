using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email
{
    public class GenerateUrl
    {
        #region public method
        /// <summary>
        /// générateur url pour l'authentification du mail 
        /// </summary>
        /// <returns></returns>
        public static string GenerationUrlAuthentification(UserManager<ApplicationUser> userManager
            , IOptions<UrlOptions> urlOptions, ApplicationUser user)
        {
            var token = userManager.GenerateEmailConfirmationTokenAsync(user).Result;
            string encodedToken = EncodingMessage(token);
            string encodedId = EncodingMessage(user.Id);

            //var url = new Uri($"https://localhost:7291/api/v1/Authenticate/ConfirmEmail/{encodedId}/{encodedToken}");
            var url = new Uri($"{urlOptions.Value.UrlEmailAuthentification}/{encodedId}/{encodedToken}");
            return url.ToString();
        }

        /// <summary>
        /// génération url pour l'authentification du mail sans définir le token dedans
        /// </summary>
        /// <param name="userManager"></param>
        /// <param name="urlOptions"></param>
        /// <param name="user"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GenerationUrlAuthentification( IOptions<UrlOptions> urlOptions, ApplicationUser user, string token )
        {
            string encodedToken = EncodingMessage(token);
            string encodedId = EncodingMessage(user.Id);

            //var url = new Uri($"https://localhost:7291/api/v1/Authenticate/ConfirmEmail/{encodedId}/{encodedToken}");
            var url = new Uri($"{urlOptions.Value.UrlEmailAuthentification}/{encodedId}/{encodedToken}");
            return url.ToString();
        }

        /// <summary>
        /// génération url pour l'authentification du mail sans définir le token dedans et prend en
        /// paramètre email 
        /// </summary>
        /// <param name="urlOptions"></param>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GenerationUrlEmailUpdate(IOptions<UrlOptions> urlOptions, ApplicationUser user, string email,string token)
        {
            string encodedToken = EncodingMessage(token);
            string encodedId = EncodingMessage(user.Id);
            string encodedEmail = EncodingMessage(email);

            var url = new Uri($"{urlOptions.Value.UrlUpdateEmail}/{encodedId}/{encodedEmail}/{encodedToken}");
            return url.ToString();
        }

        /// <summary>
        /// Génération url pour la récupération du compte lors du changement 
        /// </summary>
        /// <param name="urlOptions"></param>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GenerationUrlRecupAccount(IOptions<UrlOptions> urlOptions, ApplicationUser user, string token)
        {
            return GenerationUrlRecupAccount(urlOptions, user,  user.Email??string.Empty , token);
        }

        /// <summary>
        /// Génération url pour la récupération du compte lors du changement 
        /// </summary>
        /// <param name="urlOptions"></param>
        /// <param name="user"></param>
        /// <param name="email"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public static string GenerationUrlRecupAccount(IOptions<UrlOptions> urlOptions, ApplicationUser user, string oldMail, string token)
        {
            string encodedToken = EncodingMessage(token);
            string encodedId = EncodingMessage(user.Id);
            string encodedEmail = EncodingMessage(oldMail);

            var url = new Uri($"{urlOptions.Value.UrlRecupEmail}/{encodedId}/{encodedEmail}/{encodedToken}");
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
