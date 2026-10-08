using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Repository
{
    public interface ILocalisationSpecifiqueRepository : IRepository<Localisation>
    {
        public bool ConvertLocalisationPhotoToLocalisation(int idPhoto);
    }
}
