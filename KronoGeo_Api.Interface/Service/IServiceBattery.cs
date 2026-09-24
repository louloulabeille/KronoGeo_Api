using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceBattery
    {
        /// <summary>
        /// Retourne si le device est en économie d'énergie
        /// qui va désactiver le système de géolocalisation
        /// </summary>
        /// <returns></returns>
        public bool IsBatterySaver();
        /// <summary>
        /// Permet d'ouvrir la fenêtre de gestion de l'économie d'énergie
        /// </summary>
        public void OpenWindowBatterySaver();
    }
}
