using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class DeleteUserHandler(ILogger<DeleteUserHandler> logger,
            IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager)
        : UserIdentityHandler(logger, keyBearer, signInManager),
        IRequestHandler<DeleteUserCommand, RegisterIdentity>
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
        public async Task<RegisterIdentity> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                var user =  await _signInManager.UserManager.FindByIdAsync(request.Id);
                var registerIdentity = new RegisterIdentity() { 
                    Register = new() 
                    { 
                        Id = request.Id,
                        Login = user?.UserName ?? string.Empty,
                        Password = string.Empty,
                    } 
                };

                if (user is not null)
                {
                    // - vérification de ne pas supprimer un utilisateur avec des rôles critiques (ex: admin)
                    // - il doit en rester au moins un dans le système
                    if(!await CanDeleteUser(user))
                    {
                        var identityResult = IdentityResult.Failed(new IdentityError { Description = "Cannot delete user with critical roles." });
                        registerIdentity.IdentityResult = identityResult;
                        return registerIdentity;
                    }

                    var result = await _signInManager.UserManager.DeleteAsync(user);
                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User {Login} deleted successfully.", user.UserName);
                    }
                    registerIdentity.IdentityResult = result;

                }
                else
                {
                    var identityResult = IdentityResult.Failed(new IdentityError { Description = "User not found." });
                    registerIdentity.IdentityResult = identityResult;
                }

                return registerIdentity;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user {Login}.", ex.Message);

                // - result null ou failed avec message d'erreur
                return new RegisterIdentity() { Register = new() { Id = request.Id ,Login = string.Empty, Password = string.Empty } };
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
        public async Task<bool> CanDeleteUser(IdentityUser user)
        {
            // - vérification de ne pas supprimer un utilisateur avec des rôles critiques (ex: admin)
            var roles = await _signInManager.UserManager.GetRolesAsync(user);
            if (roles.Contains("Admin"))
            {
                var adminUsers = await _signInManager.UserManager.GetUsersInRoleAsync("Admin");
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
