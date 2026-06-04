using KronoGeo_Api.Interface.Service.MessageFactoryMethod;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod
{
    public abstract class MessageCourrielFactory
    {
        public abstract IMessageMail FactoryMethod();

        public string SomeOperation()
        {
            // Call the factory method to create a Product object.
            var message = FactoryMethod();
            // Now, use the product.
            var result = message.ReturnMessage();

            return result;
        }
    }
}
