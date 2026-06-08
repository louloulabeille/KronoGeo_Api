using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using KronoGeo_Api.Models.Infrastructure.Email;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class MessageChangeEmailCreator(UserManager<IdentityUser> userManager
        , IdentityUser user, IOptions<UrlOptions> urlOptions) : MessageCourrielFactory
    {

        #region private properties
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        #endregion

        public override IMessageMail FactoryMethod()
        {
            return new MessageChangeEmail(_userManager, _user, _urlOptions);
        }
    }
}
