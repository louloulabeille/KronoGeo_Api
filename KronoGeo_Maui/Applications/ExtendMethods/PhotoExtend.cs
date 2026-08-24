using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.Text;

#if ANDROID
    using KronoGeo_Maui.Platforms.Android.Application.Camera;
#endif

#if IOS
    using KronoGeo_Maui.Platforms.iOS.Application;
#endif

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class PhotoExtend
    {
        extension (IServiceCollection services )
        {
            public IServiceCollection AddServiceSavePhotoLocal()
            {
#if ANDROID
                services.AddTransient<IServiceSavePhotoOsDirectory, ServiceSavePhotoLocalAndroid>();
#endif
#if IOS
                services.AddTransient<IServiceSavePhotoOsDirectory,ServiceSavePhotoLocalIOS>();
#endif

                return services;
            }
        }
    }
}
