using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Repository
{
    public interface ILocalisationSpecifiqueRepository : IRepository<Localisation>
    {
        /// <summary>
        /// supprime la localisation Photo pour la transformer en localisation normale
        /// </summary>
        /// <param name="idPhoto"></param>
        /// <returns></returns>
        public bool ConvertLocalisationPhotoToLocalisation(int idPhoto);
        /// <summary>
        /// Retourne les LocalisationPhoto selon le user id et par rapport au min et max latitude
        /// </summary>
        /// <param name="idUSer"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <returns></returns>
        public Task<IEnumerable<LocalisationPhoto>> GetLocalisationPhotoByUserIdAndMinMaxLatitude(string idUSer, 
            double minLat, double maxLat, int idIgnore);
    }
}
