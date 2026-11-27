using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http.ApiExterne.Model.Api.Meteo
{
    public class ResponseApiWeather
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Generationtime_ms { get; set; }
        public int Utc_offset_seconds { get; set; }
        public string Timezone { get; set; } = string.Empty;
        public string Timezone_abbreviation { get; set; } = string.Empty;
        public float Elevation { get; set; }
        public HourlyUnit? Hourly_units { get; set; }
        public Hourly? Hourly { get; set; }
    }
}
