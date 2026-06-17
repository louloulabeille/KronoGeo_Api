using KronoGeo_Api.Interface.Service;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class InMemoryMauiParametrage : IServiceSaveParametrage
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="nameParam"></param>
        /// <param name="valueDefault"></param>
        /// <returns></returns>
        public object GetParam(string nameParam, object valueDefault)
        {
            if (valueDefault is string valueS) return Preferences.Default.Get(nameParam, valueS);
            if (valueDefault is bool valueB) return Preferences.Default.Get(nameParam, valueB);

            return valueDefault;
        }

        /// <summary>
        /// Methode Sauvegarde des paramétrages
        /// </summary>
        /// <param name="param"></param>
        public void SaveParam(string name,object param)
        {
            Preferences.Remove(name);

            if(param is string valueS) SaveParam(name ,valueS);
            if(param is bool valueB) SaveParam(name, valueB);
        }

        #region method private methoid d'enregistrement selon le type
        private static void SaveParam(string name, string param)
        {
            Preferences.Default.Set(name, param);
        }

        private static void SaveParam(string name ,bool param)
        {
            Preferences.Default.Set(name, param);
        }
        #endregion
    }
}
