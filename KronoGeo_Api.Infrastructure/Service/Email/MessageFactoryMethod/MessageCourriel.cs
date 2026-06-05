using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public static class MessageCourriel
    {
        public static string Message(MessageCourrielFactory creator)
        {
            return creator.SomeOperation();
        }
    }
}
