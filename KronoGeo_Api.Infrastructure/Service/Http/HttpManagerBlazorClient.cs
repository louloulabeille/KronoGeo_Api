using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    public class HttpManagerBlazorClient(IOptions<UrlApiBlazorClient> options , HttpClient httpClient
        , ILogger<HttpManagerBlazorClient> logger) 
        : IServiceHttpClientAssembly, IDisposable
    {
        #region private readonly properties
        private readonly IOptions<UrlApiBlazorClient> _options = options;
        private readonly HttpClient _httpClient = httpClient;
        private readonly ILogger<HttpManagerBlazorClient> _logger = logger;
        #endregion

        #region public method interface IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion



    }
}
