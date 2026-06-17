using System;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceSaveParametrage
    {
        public void SaveParam(string name,object param);
        public object GetParam(string nameParam, object valueDefault);
    }
}
