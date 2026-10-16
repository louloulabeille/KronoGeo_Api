using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Applications.MediatR.Queries.Identity;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Email;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Manager
{
    public class UpdateUserPasswordHandler(ILogger<UpdateUserPasswordHandler> logger
        , SignInManager<ApplicationUser> signInManager) 
        : ManagerHandler(logger, signInManager)
        , IRequestHandler<UpdateUserPasswordCommand, ResponseApiManager>
    {
        /// <summary>
        /// update le mot de passe - en vérifiant le nouveau comme l'ancien 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<ResponseApiManager> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.Profil.Password) || string.IsNullOrEmpty(request.Profil.NewPassword))
                { 
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = "Password not found or empty" };
                }

                var user = await _signInManager.UserManager.FindByIdAsync(request.Profil.Id ?? string.Empty) ??
                    await _signInManager.UserManager.FindByNameAsync(request.Profil.Login??string.Empty);

                if ( user is not null )
                {
                    // -- method pour changer le mot passe - il faut l'ancien mot de passe
                    var identityResult = await _signInManager.UserManager.ChangePasswordAsync(user, request.Profil.Password, request.Profil.NewPassword);
                    if ( identityResult.Succeeded )
                    {
                        return new ResponseApiManager { ApiStatus = EnumApiStatus.Success };
                    }
                    else
                    {
                        return new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = "Impossible to update password" };
                    }
                }
                else
                { // -- impossible de trouver l'utilisateur
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.NotFound, Message = "User not found" };
                }

            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while processing change password for user {Login}.", request.Profil.Login);
                //result.IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Internal Error." });
                return new ResponseApiManager { ApiStatus = EnumApiStatus.Problem, Message = "Internal Error." };
            }
        }
    }
}
