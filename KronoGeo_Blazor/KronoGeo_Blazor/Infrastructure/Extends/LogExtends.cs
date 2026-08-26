using Serilog;

namespace KronoGeo_Blazor.Infrastructure.Extends
{
    public static class LogExtends
    {
        extension (IHostBuilder host)
        {
            /// <summary>
            /// method qui configure Serilog pour l'enregistrement des logs dans la console
            /// et dans des fichiers de log quotidiens
            /// </summary>
            /// <returns></returns>
            public IHostBuilder AddSeriLog()
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($"logs/{DateTime.Now:yyyy}/{DateTime.Now:MM}/log-{DateTime.Now:dd-MM-yyyy}.txt",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();
                host.UseSerilog();
                return host;
            }
        }
    }
}
