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
    public class MessageAuthentificationCreator(UserManager<ApplicationUser> userManager
        , ApplicationUser user, IOptions<UrlOptions> urlOptions) : MessageCourrielFactory
    {
        #region private properties
        private readonly UserManager<ApplicationUser> _userManager = userManager;
        private readonly ApplicationUser _user = user;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        #endregion

        public override IMessageMail FactoryMethod()
        {
            return new MessageAuthentification(_userManager, _user, _urlOptions);
        }
    }
}
