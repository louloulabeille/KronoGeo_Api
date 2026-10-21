using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
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
                {
                    var roles = await SignInManager.UserManager.GetRolesAsync(user);
                    // -- récupération des users de la base de données
                    if (roles.ToList().Exists(r => r == "Admin" || r == "Manager"))
                    {
                        ResponseApiManager response = new () {
                            ApiStatus = EnumApiStatus.Success,
                            Profils = []
                        };

                        // -- role User
                        var users = await SignInManager.UserManager.GetUsersInRoleAsync("User");
                        foreach(var item in users)
                        {
                            var roleItem = await SignInManager.UserManager.GetRolesAsync(item);
                            var profil = new ProfilDTO
                            {
                                Id = item.Id,
                                Login = item.UserName,
                                Email = item.Email,
                                PhoneNumber = item.PhoneNumber,
                                Roles = roleItem.ToList()
                            };
                            response.Profils?.Add(profil);

                            // -- éviter de mettre les utilisateurs Admin pour les Manager
                            if ( roles.ToList().Exists(r=> r=="Manager" ) 
                                && roleItem.ToList().Exists(rI => rI == "Admin"))
                            {
                                response.Profils?.Remove(profil);
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
