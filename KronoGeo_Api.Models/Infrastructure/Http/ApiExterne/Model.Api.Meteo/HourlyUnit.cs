using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http.ApiExterne.Model.Api.Meteo
{
    public class HourlyUnit
    {
        public string Time { get; set; } = "iso8601";
        public string Temperature_2m { get; set; } = "°C";
        public string Relative_humidity_2m { get; set; } = "%";
        public string Dew_point_2m { get; set; } = "°C";
        public string Apparent_temperature { get; set; } = "°C";
        public string Wind_speed_10m { get; set; } = "km/h";
        public string Precipitation { get; set; } = "mm";
        public string Surface_pressure { get; set; } = "hPa";
        public string Weather_code { get; set; } = "wmo code";
    }
}
