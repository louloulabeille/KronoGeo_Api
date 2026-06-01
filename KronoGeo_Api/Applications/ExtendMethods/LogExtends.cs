using Serilog;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class LogExtends
    {
        extension(IHostBuilder host)
        {
            /// <summary>
            /// method qui configure Serilog pour l'enregistrement des logs dans la console
            /// et dans des fichiers de log quotidiens
            /// </summary>
            /// <returns></returns>
            public IHostBuilder AddSerilog()
            {
                Log.Logger = new LoggerConfiguration()
                    .WriteTo.Console()
                    .WriteTo.File($"logs/log-{DateTime.Now:dd-MM-yyyy}.txt", 
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:dd-MM-yyyy HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}"
                    )
                    .CreateLogger();
                host.UseSerilog();
                return host;
            }
        }
    }
}
