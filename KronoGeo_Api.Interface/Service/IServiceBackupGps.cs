using KronoGeo_Api.Models;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceBackupGps
    {
        /// <summary>
        /// contante du nom du fichier de backup
        /// </summary>
        public const string Filename = "Gps_BackUpKrono.json";
        
        #region method public de l'interface
        /// <summary>
        /// sauvegarde la liste de location dans le fichier de backup
        /// </summary>
        /// <param name="points"></param>
        public void SavePointsLocalisation(List<Localisation> points);
        /// <summary>
        /// retourne la liste de location qui se trouve le fichier de backup
        /// </summary>
        /// <returns></returns>
        public List<Localisation>? ReturnLocalisation();
        /// <summary>
        /// method pour vérifier si le fichier de back existe déjà
        /// </summary>
        /// <returns></returns>
        public bool FileExist();
        /// <summary>
        /// supprime le fichier de backup
        /// </summary>
        public void DeleteFile();
        #endregion
    }
}
