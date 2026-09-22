using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class ParametrageExtends
    {
        extension( IServiceCollection services )
        {
#if ANDROID
            public IServiceCollection AddListOsDeviceBatteryGestion(IConfiguration config)
            {
                services.AddOptions();
                services.Configure<AndroidOSListBattery>(config.GetSection("DeviceAndroidBattery"));
                return services;
            }
#endif
        }
    }
}
