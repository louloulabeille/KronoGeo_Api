using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Manager
{
    public class UpdateUserRoleHandler(SignInManager<ApplicationUser> signInManager
        , ILogger<UpdateUserRoleHandler> logger, IOptions<KeyBearer> keyBearer)
        : ManagerHandler(logger, signInManager, keyBearer)
        , IRequestHandler<UpdateUserCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await SignInManager.UserManager.FindByIdAsync(request.Profil.Id ?? string.Empty)
                    ?? await SignInManager.UserManager.FindByEmailAsync(request.Profil.Email ?? string.Empty);

                if (user is not null && request.Profil.Roles is not null)
                {
                    var roles = await SignInManager.UserManager.GetRolesAsync(user);
                    List<string> addRoles = request.Profil.Roles.Except(roles).ToList();
                    List<string> removeRoles = roles.Except(request.Profil.Roles).ToList();

                    if ( addRoles is not null)
                        await SignInManager.UserManager.AddToRolesAsync(user, addRoles);
                    if (removeRoles is not null)
                        await SignInManager.UserManager.RemoveFromRolesAsync(user, removeRoles);
                    return new ResponseApiManager()
                    {
                        ApiStatus = EnumApiStatus.Success,
                        Message = "User roles updated successfully."
                    };
                }
                else
                {
                    return new ResponseApiManager()
                    {
                        ApiStatus = EnumApiStatus.NotFound,
                        Message = "User not found.",
                        Profil = request.Profil
                    };
                }
            }
            catch(Exception ex)
            {
                Logger.LogError(ex, "Erreur lors de la modification des roles pour l'utilisateur {id}" +
                    ", message erreur {message}"
                    , request.Profil.Id, ex.Message);
                return new ResponseApiManager()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "An error occurred while updating roles user"
                };
            }
        }
    }
}
