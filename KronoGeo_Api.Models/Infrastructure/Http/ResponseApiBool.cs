using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public class ResponseApiBool: ResponseApi
    {
        public bool Retour { get; set; } = false;
    }
}
