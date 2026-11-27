using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Http.ApiExterne.Model.Api.Meteo;
using KronoGeo_Api.Models.Infrastructure.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceApiMeteo
    {
        public Task<ResponseApiWeather?> SendAsync(double latitude, double longitude);
    }
}
