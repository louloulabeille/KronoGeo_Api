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
        , IdentityUser user, IOptions<UrlOptions> urlOptions, string lienToken) : IMessageMail
    {
        #region private properties
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        private readonly string _lienToken = lienToken;
        #endregion

        public string ReturnMessage()
        {
            
            StringBuilder message = new();
            message.AppendLine($"<h1>Bonjour {_user.UserName} </h1>");
            message.AppendLine("<p>Pour valider le changment de votre adresse mail sur ce compte , vous devez valider ce lien : </p>");
            message.AppendLine($"<a href='{_lienToken}' target='_blank'>Confirmer mon adresse email</a>");
            message.AppendLine("<p>Attention : ce lien expire dans 24 heures. Si vous n'avez jamais demandé la modification de votre email, vous allez recevoir un lien pour récupérer votre adresse mail. Faites un changement de mot de passe si votre compte a été piraté.</p>");
            message.AppendLine("<p>Cordialement,</p>");
            message.AppendLine("");
            message.AppendLine($"ps:");
            message.AppendLine($"Si vous avez un problème avec le lien de confirmation, vous pouvez copier ce lien dans votre navigateur.");
            message.AppendLine($"{_lienToken}");
            return message.ToString();
        }
    }
}
