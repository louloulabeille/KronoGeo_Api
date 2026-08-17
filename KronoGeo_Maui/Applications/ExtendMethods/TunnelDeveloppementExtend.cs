using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class TunnelDeveloppementExtend
    {
        extension(IServiceCollection services)
        {

            /// <summary>
            /// IOptions TokenTunnel config chargemennt du token 
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            public IServiceCollection AddTokenTunnelDeveloppement(IConfiguration config)
            {
                services.AddOptions();
                services.Configure<TokenTunnel>(config.GetSection("TokenTunnel"));
                return services;
            }

        }
    }
}
