using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceSaveLocalisation
    {
        public Task<bool> SaveLocalisation(LocalisationGroup localisations, CancellationToken cancellationToken);
    }
}
 