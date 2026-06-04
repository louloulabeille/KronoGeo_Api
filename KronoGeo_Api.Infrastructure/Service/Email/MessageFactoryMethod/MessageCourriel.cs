using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public static class MessageCourriel
    {
        public static string Message(MessageAuthentificationCreator creator)
        {
            return creator.SomeOperation();
        }
    }
}
