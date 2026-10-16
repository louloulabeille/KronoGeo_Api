using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public class ResponseApiManager : ResponseApi
    {
        public ProfilDTO? Profil { get; set; } = default;
        // -- liste des roles
        public List<string>? Roles { get; set; }
    }
}
