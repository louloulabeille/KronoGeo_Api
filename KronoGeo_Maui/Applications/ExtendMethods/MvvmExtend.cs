using KronoGeo_Maui.ModelViews;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class MvvmExtend
    {
        extension ( IServiceCollection services )
        {
            public IServiceCollection AddMvvmInjection()
            {
                // - MainPage
                services.AddSingleton<MainPage>();
                services.AddSingleton<MainPageViewModel>();

                return services;
            }
        }
    }
}
