using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service.MessageFactoryMethod
{
    // This interface defines a contract for message mail,
    // which includes a method to return a message string.
    public interface IMessageMail
    {
        public string ReturnMessage();
    }
}
