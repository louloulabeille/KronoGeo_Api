using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public class ResponseApiLocalisations : ResponseApi
    {
        public LocalisationGroupDTO? LocalisationGroupDTO { get; set; } = default;
        public List<LocalisationGroupDTO> GroupsDTO { get; set; } = [];
    }
}
