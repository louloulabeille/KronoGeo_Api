using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http.ApiExterne.Model.Api.Meteo;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using static System.Net.WebRequestMethods;

namespace KronoGeo_Api.Infrastructure.Service.Http.Meteo
{
    public class HttpClientOpenMeteo(IOptions<ParamMeteo> options
        , HttpClient httpClient) : IServiceApiMeteo, IDisposable
    {
        #region private readonly properties
        private readonly IOptions<ParamMeteo> _options = options;
        private readonly HttpClient _httpClient = httpClient;
        #endregion

        #region public method interface IServiceApiMeteo
        public async Task<ResponseApiWeather?> SendAsync(double latitude, double longitude)
        {
            try
            {
                var param = _options.Value;
                param.Latitude = latitude;
                param.Longitude = longitude;
                string url = param.ToString();

                using var response = await _httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode(); // -- lève une exception si result n'est pas un success

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<ResponseApiWeather>(result, JsonOptions.GetJsonOptions());
                    return data ?? null;
                    // -- Marche :)
                }
                return null;
            }
            catch(Exception)
            {
                return null;
            }
        }
        #endregion

        #region public method interface IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
