using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public class ResponseApiImage : ResponseApi
    {
        public PhotoDTO? PhotoDTO { get; set; } = default;
    }
}
