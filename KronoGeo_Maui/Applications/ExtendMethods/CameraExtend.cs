#if ANDROID
    using KronoGeo_Maui.Platforms.Android.Application.Camera;
#endif
using CommunityToolkit.Maui;
using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Maui.Applications.Interface;
using KronoGeo_Maui.Applications.Services.Camera;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class CameraExtend 
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddServiceCamera()
            {
#if ANDROID
                // on ne l'utilise plus on prend le system de photo du téléphone
                // -- injection du service de prise de photo pour android
                //services.AddTransient<IServiceCamera, PhotoAndroid>();
#endif
                services.AddTransient<IServiceCamera, ServicePhoto>();
                return services;
            }
        }
    }
}
