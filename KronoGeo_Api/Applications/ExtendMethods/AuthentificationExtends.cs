using KronoGeo_Api.Infrastructure.Database;
using Microsoft.AspNetCore.Identity;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class AuthentificationExtends
    {
        extension (IServiceCollection services)
        {
            /// <summary>
            /// Mise en place du paramétrage par défaut de IdentityUser
            /// par exemple la taille du mot de passe s'il faut un mail de confirmation
            /// etc 
            /// installer le framework Identity.Ui
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddCustonIdentityUser()
            {
                services.AddDefaultIdentity<IdentityUser>(options =>
                {
                    options.Password = new PasswordOptions()
                    {
                        RequiredLength = 12,
                        RequireUppercase = true,
                        RequiredUniqueChars = 1,
                        RequireLowercase = true,
                        RequireDigit = true,
                        RequireNonAlphanumeric = true,
                    };

                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789-._@+";
                    options.User.RequireUniqueEmail = true;

                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 3;

                    // - a mettre en place après
                    //options.SignIn.RequireConfirmedEmail = true;
                    //options.SignIn.RequireConfirmedAccount = true;
                })
                    .AddEntityFrameworkStores<KronoGeoDbContext>();

                return services;
            }
        }
    }
}
