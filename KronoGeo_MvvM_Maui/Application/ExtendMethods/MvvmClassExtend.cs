using KronoGeo_MvvM_Maui.ViewModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_MvvM_Maui.Application.ExtendMethods
{
    public static class MvvmClassExtend
    {
        extension( IServiceCollection services)
        {
            public void AddMvvmClass()
            {
                // - MVVM pour la MainPage
                services.AddSingleton<MainPage>();
                services.AddSingleton<MainPageViewModel>();

                // - 
            }
        }
    }
}
