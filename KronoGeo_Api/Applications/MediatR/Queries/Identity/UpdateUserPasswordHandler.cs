using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Email;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class UpdateUserPasswordHandler(ILogger<UpdateUserPasswordHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<ApplicationUser> signInManager) 
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<UpdateUserPasswordCommand, RegisterIdentity>
    {
        /// <summary>
        /// update le mot de passe - en vérifiant le nouveau comme l'ancien 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RegisterIdentity> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = new RegisterIdentity { Register = request.Register };

            try
            {
                if (string.IsNullOrEmpty(request.Register.Password) || string.IsNullOrEmpty(request.Register.NewPassord))
                {
                    result.IdentityResult = IdentityResult.Failed(new IdentityError() { Description = "Password failed." });
                    return result;
                }

                var user = await _signInManager.UserManager.FindByIdAsync(request.Register.Id)?? 
                    await _signInManager.UserManager.FindByNameAsync(request.Register.Login);

                if ( user is not null )
                {
                    var identityResult = await _signInManager.UserManager.ChangePasswordAsync(user, request.Register.Password, request.Register.NewPassord);
                    result.IdentityResult = identityResult;
                }
                else
                {
                    result.IdentityResult = IdentityResult.Failed(new IdentityError() { Description = "User not found." });
                }

            }
            catch (Exception ex) {
                _logger.LogError(ex, "An error occurred while processing change password for user {Login}.", result.Register.Login);
                result.IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Internal Error." });
            }
            
            return result;
        }
    }
}
