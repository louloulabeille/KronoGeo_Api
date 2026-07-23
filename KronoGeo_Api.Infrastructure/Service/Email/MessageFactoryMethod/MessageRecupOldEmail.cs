using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class MessageRecupOldEmail(UserManager<ApplicationUser> userManager
        , IdentityUser user, IOptions<UrlOptions> urlOptions, string lienToken) : IMessageMail
    {
        #region private properties
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        private readonly string _lienToken = lienToken;
        #endregion

        public string ReturnMessage()
        {
            StringBuilder message = new();
            message.AppendLine($"<h1>Bonjour {_user.UserName} </h1>");
            message.AppendLine("<p>Mail de récupération de votre compte en cas de changement d'adresse mail. Le lien ci-dessous sert à récupérer votre ancienne adresse mail pour votre compte. Changez votre mot de passe si votre compte a été piraté.</p>");
            message.AppendLine($"<a href='{_lienToken}' target='_blank'>Confirmer mon adresse email</a>");
            message.AppendLine("<p>Attention : ce lien expire dans 24 heures. Ne prennez pas en compte ce mail, si le changment d'adresse mail est correct.</p>");
            message.AppendLine("<p>Cordialement,</p>");
            message.AppendLine("");
            message.AppendLine($"ps:");
            message.AppendLine($"Si vous avez un problème avec le lien de confirmation, vous pouvez copier ce lien dans votre navigateur.");
            message.AppendLine($"{_lienToken}");
            return message.ToString();
        }
    }
}
