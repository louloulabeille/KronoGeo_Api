using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Manager
{
    public class ProfilAllUsersHandler(ILogger<ProfilAllUsersHandler> logger
        , SignInManager<ApplicationUser> signInManager, IOptions<KeyBearer> keyBearer)
        : ManagerHandler(logger, signInManager, keyBearer)
        , IRequestHandler<ProfilAllUsersCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(ProfilAllUsersCommand request, CancellationToken cancellationToken)
        {
            try
            {
                
                if (string.IsNullOrEmpty(request.IdUser))
                {
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = "Bad request." };
                }

                var user = await SignInManager.UserManager.FindByIdAsync(request.IdUser ?? string.Empty);

                if (user is null)
                {
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.NotFound, Message = "User not found" };
                }
                else
                {   // -- récupération du role de l'utilisateur 
                    var roles = await SignInManager.UserManager.GetRolesAsync(user);
                    
                    if (roles.ToList().Exists(r => r == "Admin" || r == "Manager"))
                    {
                        ResponseApiManager response = new () {
                            ApiStatus = EnumApiStatus.Success,
                            Profils = []
                        };

                        // -- récupération des utilisateurs
                        var users = await SignInManager.UserManager.Users.AsNoTracking().ToListAsync();
                        //var result = new List<ProfilDTOAdmin>();

                        // -- recherche des roles et si enregistrement dans la liste
                        // -- si role Manager il ne peut pas accéder aux compte admin

                        foreach (var u in users)
                        {
                            var rolesUtil = await SignInManager.UserManager.GetRolesAsync(u);
                            var profil = new ProfilDTOAdmin
                            {
                                Id = u.Id,
                                Login = u.UserName,
                                Email = u.Email,
                                Roles = rolesUtil.ToList()
                            };
                            response.Profils.Add(profil);

                            // -- pour éviter de donner la possibilité au manager de voir les admin
                            // -- et de pouvoir les modifier
                            if (roles.ToList().Exists(r => r == "Manager")
                                && rolesUtil.ToList().Exists(rI => rI == "Admin"))
                            {
                                response.Profils.Remove(profil);
                            }
                            
                        }

                        return response;
                    }
                    else
                    {
                        Logger.LogCritical("Utilisateur qui ne devrait pas être ici pas les droits {id} ", request.IdUser);
                        return new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = "Unauthorized user." };
                    }

                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing return user {id}.", request.IdUser);
                return new ResponseApiManager { ApiStatus = EnumApiStatus.Problem, Message = "An error occurred while processing return user" };
            }


            
        }
    }
}
