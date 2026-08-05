using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class InApiSaveLocalisation : IServiceSaveLocalisation
    {
        #region public method Interface IServiceSaveLocalisation
        /// <summary>
        /// sauvegarde les localisations dans l'API
        /// </summary>
        /// <param name="localisations"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Task<bool> SaveLocalisation(LocalisationGroup localisations, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
        #endregion

        #region private method

        #endregion
    }
}
