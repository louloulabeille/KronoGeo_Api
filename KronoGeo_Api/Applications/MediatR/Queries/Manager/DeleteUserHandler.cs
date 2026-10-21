using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Applications.MediatR.Queries.Manager;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class DeleteUserHandler(ILogger<DeleteUserHandler> logger
        , SignInManager<ApplicationUser> signInManager, IOptions<KeyBearer> keyBearer)
        : ManagerHandler(logger, signInManager, keyBearer),
        IRequestHandler<DeleteUserCommand, ResponseApiManager>
    {

        #region handle IRequestHandler
        /// <summary>
        /// method qui permet de supprimer un utilisateur en fonction de son login ou email,
        /// avec des vérifications pour éviter de supprimer des utilisateurs critiques (ex: admin) 
        /// et pour s'assurer qu'il reste au moins un utilisateur dans le système.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseApiManager> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            var profilManager = new ResponseApiManager();
            try
            {
                var user =  await SignInManager.UserManager.FindByIdAsync(request.Id);

                if (user is not null)
                {
                    // - vérification de ne pas supprimer un utilisateur avec des rôles critiques (ex: admin)
                    // - il doit en rester au moins un dans le système
                    if(!await CanDeleteUser(user))
                    {
                        profilManager.ApiStatus = EnumApiStatus.BadRequest;
                        profilManager.Message = "Cannot delete user with critical roles.";
                        return profilManager;
                    }

                    var result = await SignInManager.UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        //_logger.LogInformation("User {Login} deleted successfully.", user.UserName);
                        profilManager.ApiStatus = EnumApiStatus.Success;
                    }
                    else
                    {
                        profilManager.ApiStatus = EnumApiStatus.BadRequest;
                        profilManager.Message = "Impossible to delete user";
                    }

                }
                else
                {
                    profilManager.ApiStatus = EnumApiStatus.NotFound;
                    profilManager.Message = "User not found.";
                }

                return profilManager;
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while deleting user {Login}.", ex.Message);

                profilManager.ApiStatus = EnumApiStatus.Problem;
                profilManager.Message = "An error occurred while deleting user";
                // - result null ou failed avec message d'erreur
                return profilManager;
            }
            
        }
        #endregion

        #region private methods
        /// <summary>
        /// Vérifie si un utilisateur peut être supprimé en fonction de ses rôles 
        /// et du nombre d'utilisateurs dans ces rôles.
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        public async Task<bool> CanDeleteUser(ApplicationUser user)
        {
            // - vérification de ne pas supprimer un utilisateur avec des rôles critiques (ex: admin)
            var roles = await SignInManager.UserManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                var adminUsers = await SignInManager.UserManager.GetUsersInRoleAsync("Admin");
                if (adminUsers.Count <= 1)
                {
                    return false; // - ne pas supprimer le dernier admin
                }
            }
            return true;
        }

        #endregion
    }
}
