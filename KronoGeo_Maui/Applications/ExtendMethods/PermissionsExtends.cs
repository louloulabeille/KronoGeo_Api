using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class PermissionsExtends
    {
        extension ( IServiceCollection services )
        {
            public IServiceCollection AddPackgeNameAndroid(IConfiguration config)
            {
                services.AddOptions();
                services.Configure<PackageNameAndroid>(config.GetSection("Application"));

                return services;
            }
        }
    }
}
