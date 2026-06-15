using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{

    /// <summary>
    /// class de retour 
    /// </summary>
     public class ResponseApiAuthenticate : ResponseApi
    {
        public RegisterDTO? Register { get; set; }
    }
}
