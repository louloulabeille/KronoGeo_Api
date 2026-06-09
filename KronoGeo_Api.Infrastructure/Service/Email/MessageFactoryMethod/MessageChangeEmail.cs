using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class MessageChangeEmail(UserManager<IdentityUser> userManager
        , IdentityUser user, IOptions<UrlOptions> urlOptions) : IMessageMail
    {
        #region private properties
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        //private readonly string _lienToken = lienToken;
        #endregion

        public string ReturnMessage()
        {
            string lien = GenerateUrl.GenerationUrlAuthentification(_userManager, _urlOptions, _user);

            StringBuilder message = new();
            message.AppendLine($"<h1>Bonjour {_user.UserName} </h1>");
            message.AppendLine("<p>Vous avez changé votre adresse email. Pour le valider veuillez cliquer sur le lien suivant pour confirmer votre adresse email : </p>");
            message.AppendLine($"<a href='{lien}' target='_blank'>Confirmer mon adresse email</a>");
            message.AppendLine("<p>Attention : ce lien expire dans 72 heures.</p>");
            message.AppendLine("<p>Cordialement,</p>");
            message.AppendLine("");
            message.AppendLine($"ps:");
            message.AppendLine($"Si vous avez un problème avec le lien de confirmation, vous pouvez copier ce lien dans votre navigateur.");
            message.AppendLine($"{lien}");
            return message.ToString();
        }
    }
}
