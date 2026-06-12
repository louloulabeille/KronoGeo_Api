using KronoGeo_Api.Applications.CustomTokenProviders;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.Security.Claims;
using System.Text;

namespace KronoGeo_Api.Applications.ExtendMethods
{
    public static class AuthentificationExtends
    {
        //private const string _emailConfirmationTokenProviderName = "ConfirmEmail";

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

                    options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789²&~#{[|`\\^@]}'()-_=?;.:/!";
                    options.User.RequireUniqueEmail = true;
                    options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromMinutes(10);
                    options.Lockout.MaxFailedAccessAttempts = 3;

                    // - a mettre en place après
                    options.SignIn.RequireConfirmedEmail = true;
                    //options.SignIn.RequireConfirmedAccount = true;
                    // - format du token pouvant être configuré
                    //options.Tokens.EmailConfirmationTokenProvider = TokenOptions.DefaultEmailProvider;
                    options.Tokens.EmailConfirmationTokenProvider = "EmailConfirmation";
                })
                    .AddRoles<IdentityRole>() // - permet la mise en place de rôles pour les utilisateurs
                    .AddEntityFrameworkStores<KronoGeoDbContext>()
                    .AddTokenProvider<EmailConfirmationTokenProvider<IdentityUser>>("EmailConfirmation")
                    .AddDefaultTokenProviders(); // - mise en place de la vérification du token

                // - configuration de la durée de vie du token 24h par défaut 24h
                services.Configure<DataProtectionTokenProviderOptions>(options => {
                    options.TokenLifespan = TimeSpan.FromHours(24);
                });

                // - durée de vie pour les tokens de validations email 3 jours 
                services.Configure<EmailConfirmationTokenProviderOptions>(options =>
                {
                    options.TokenLifespan = TimeSpan.FromDays(3);
                });

                return services;
            }

            /// <summary>
            /// installer le framework Authentification Jwt-bearer 
            /// PAramétrage du Token de sécurité pour l'authentification et la validation du token
            /// </summary>
            /// <param name="config"></param>
            /// <returns></returns>
            public IServiceCollection AddCustomlsAuthentification(IConfiguration config)
            {
                // récupération de la key de chiffrement qui est dans le le settings
                //string key = config["Key:Symetrique"]?? string.Empty;
                KeyBearer? cle = new();
                config.GetSection("Jwt").Bind(cle);

                // - ajout dans le services Ioptions de Keybearer en injection de dépendance
                // pour pouvoir l'utiliser dans les controllers ou autres services
                services.AddOptions<KeyBearer>().Bind(config.GetSection("Jwt"));

                if (string.IsNullOrEmpty(cle.Key))
                    throw new InvalidOperationException("Bearer key is not configured.");

                // - configuration de l'authentification Jbearer
                services.AddAuthentication(options => {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                }).AddJwtBearer(options => {
                    // - configuration de la validation du token
                    options.SaveToken = true;
                    options.TokenValidationParameters = new TokenValidationParameters()
                    {
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(cle.Key)),
                        ValidateAudience = cle.ValidateAudience,
                        ValidateIssuer = cle.ValidateIssuer,
                        ValidateActor = cle.ValidateActor, // - valider l'acteur qui est à l'origine de la demande d'authentification OAuth2.0
                        ValidateLifetime = cle.ValidateLifetime,    // durée de vie à paramétrer lors de la création du token envoyer vers l'user
                    };

                    // - mise en place du control de SecurityStamp pour mieux protéger le compte
                    // - si le compte est modifié le SecurityStamp est automatiquement changé dans la base de donnée
                    // - le token ne sera plus valid
                    options.Events = new JwtBearerEvents()
                    {
                        OnTokenValidated = async tokenValidateContext =>
                        {
                            var userManager     = tokenValidateContext.HttpContext.RequestServices.GetRequiredService<UserManager<IdentityUser>>();
                            var claimPrincipal  = tokenValidateContext.Principal;
                            var idUser          = claimPrincipal?.FindFirstValue("Id");
                            var securityStamp = claimPrincipal?.FindFirstValue("SecurityStamp");
                            var user = await userManager.FindByIdAsync(idUser??string.Empty);
                            if ( user is null )
                            {
                                tokenValidateContext.Fail("User not found");
                                return;
                            }
                            if ( !string.IsNullOrEmpty(user!.SecurityStamp) && user.SecurityStamp != securityStamp ){
                                tokenValidateContext.Fail("Token has been invalidated. Please log in again.");
                                return;
                            }
                        }
                    };
                });

                return services;
            }

            /// <summary>
            /// ajout des roles et claims 
            /// pour la gestion de l'autorisation au niveau des controllers ou des actions
            /// </summary>
            /// <returns></returns>
            public IServiceCollection AddAuthorizationPolicy()
            {
                services.AddAuthorizationBuilder()
                    .AddPolicy("ZoneAdmin", policy => policy.RequireClaim("Admin", "Manager"))
                    .AddPolicy("ZoneUser", policy => policy.RequireClaim("User"));
                return services;
            }
        }

        extension(WebApplication app)
        {
            /// <summary>
            /// method qui ajoute les roles par défaut à la base de données au démarrage 
            /// de l'application
            /// </summary>
            /// <returns></returns>
            public async Task<WebApplication> InitializeRolesAsync()
            {
                using (var scope = app.Services.CreateScope())
                {
                    var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
                    string[] roles = ["Admin", "Manager", "User"];

                    foreach (var role in roles)
                    {
                        if (!await roleManager.RoleExistsAsync(role))
                        {
                            await roleManager.CreateAsync(new IdentityRole(role));
                        }
                    }
                }
                return app;
            }
        }

    }
}
