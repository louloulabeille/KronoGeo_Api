using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public class MessageAuthentificationCreator(UserManager<IdentityUser> userManager
        , IdentityUser user) : MessageCourrielFactory
    {
        #region private properties
        private readonly UserManager<IdentityUser> _userManager = userManager;
        private readonly IdentityUser _user = user;
        #endregion

        public override IMessageMail FactoryMethod()
        {
            return new MessageAuthentification(_userManager, _user);
        }
    }
}
