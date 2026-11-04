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
        , IRequestHandler<UpdateUserRoleCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(UpdateUserRoleCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user = await SignInManager.UserManager.FindByIdAsync(request.Profil.Id ?? string.Empty)
                    ?? await SignInManager.UserManager.FindByEmailAsync(request.Profil.Email ?? string.Empty);

                // -- si l'utilisateur est un manager,
                // -- il ne peut pas modifier les roles d'un utilisateur qui est admin ou manager
                if (user is not null && request.Profil.Roles is not null)
                {
                    var userRoles = await SignInManager.UserManager.GetRolesAsync(user);
                    if (request.isManager)
                    {
                        // Check if the user is trying to modify admin
                        if (userRoles.Contains("Admin") )
                        {
                            return new ResponseApiManager()
                            {
                                ApiStatus = EnumApiStatus.BadRequest,
                                Message = "You are not allowed to modify the roles of an admin."
                            };
                        }

                        if( request.Profil.Roles.Contains("Admin") )
                        {
                            return new ResponseApiManager()
                            {
                                ApiStatus = EnumApiStatus.BadRequest,
                                Message = "You are not authorized to edit the Admin user."
                            };
                        }
                    }

                    // -- récupération des rôles à ajouter et à supprimer
                    List<string> addRoles = request.Profil.Roles.Except(userRoles).ToList();
                    List<string> removeRoles = userRoles.Except(request.Profil.Roles).ToList();

                    // -- mise à jour des rôles de l'utilisateur
                    if ( addRoles is not null && addRoles.Count > 0)
                        await SignInManager.UserManager.AddToRolesAsync(user, addRoles);
                    if ( !await CanDeleteUser(removeRoles))
                    {
                        return new ResponseApiManager()
                        {
                            ApiStatus = EnumApiStatus.BadRequest,
                            Message = "Impossible to delete this admin role when there's only one left."
                        };
                    }

                    if (removeRoles is not null && removeRoles.Count > 0 )
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

        #region private methods
        /// <summary>
        /// retourne s'il est possible de supprimer un role admin à un utilisateur
        /// </summary>
        /// <param name="removeRoles"></param>
        /// <returns></returns>
        private async Task<bool> CanDeleteUser(List<string> removeRoles)
        {
            // Vérification si l'utilisateur a des rôles critiques (ex: admin)
            if (removeRoles.Contains("Admin"))
            {
                // Vérification s'il reste au moins un utilisateur avec le rôle critique
                var usersWithCriticalRole = await SignInManager.UserManager.GetUsersInRoleAsync("Admin");
                if (usersWithCriticalRole.Count <= 1)
                {
                    return false; // Ne peut pas supprimer le dernier utilisateur avec un rôle critique
                }
            }
            return true; // Peut supprimer l'utilisateur
        }
        #endregion
    }
}
