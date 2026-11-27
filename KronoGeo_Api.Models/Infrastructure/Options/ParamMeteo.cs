using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Options
{
    public class ParamMeteo
    {
        public double? Latitude { get; set; }
        public double? Longitude { get; set; }
        public string[] Hourly { get; set; } = [];
        public string Models { get; set; } = "meteofrance_seamless";
        public string Timezone { get; set; } = "Europe/Paris";
        public int Forecast_days { get; set; } = 1;

        public override string ToString()
        {
            var query = new List<string>
            {
                $"latitude={this.Latitude?.ToString(CultureInfo.InvariantCulture)}",
                $"longitude={this.Longitude?.ToString(CultureInfo.InvariantCulture)}",
                $"hourly={string.Join(",", this.Hourly)}",
                $"models={this.Models}",
                $"timezone={Uri.EscapeDataString(this.Timezone)}",
                $"forecast_days={this.Forecast_days}"
            };

            return "?" + string.Join("&", query); ;
        }
    }
}
