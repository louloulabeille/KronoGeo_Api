using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    public static class SqliteExtends
    {
        extension(IServiceCollection services)
        {
            public IServiceCollection AddServiceDatabaseMaui(IConfiguration configuration)
            {
                // -- ajout de la configuration de l'option SqliteOptions
                services.AddOptions();
                services.Configure<SqliteOptions>(configuration.GetSection("Sqlite"));

                // -- injection du service de base de données
                services.AddSingleton<IServiceDatabaseMaui, DatabaseSqliteMaui>();
                return services;
            }
        }
    }
}
