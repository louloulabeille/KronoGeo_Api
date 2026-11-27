using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class MeteoDTO
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Temperature_2m { get; set; } = string.Empty;
        
    }
}
