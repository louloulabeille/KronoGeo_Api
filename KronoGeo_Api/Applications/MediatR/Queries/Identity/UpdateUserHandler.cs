using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Infrastructure.Test;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class UpdateUserHandler( ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager
        , IServiceSendMessage serviceSendMail
        , IOptions<UrlOptions> urlOptions)
        : UserIdentityHandler (logger, keyBearer, signInManager)
        //, IRequestHandler<UpdateUserCommand, RegisterIdentity>
    {


        #region private properties
        private readonly IServiceSendMessage _serviceSendMail = serviceSendMail;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        #endregion

        #region public method
        /// <summary>
        /// method qui met à jour le compte user identity
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RegisterIdentity> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            RegisterIdentity result = new() { Register = request.Register };
            bool isChangeMail = false;
            try
            {
                var user = await _signInManager.UserManager.FindByIdAsync(request.Register.Id) ??
                await _signInManager.UserManager.FindByNameAsync(request.Register.Login);

                if (user is null) return new RegisterIdentity()
                {
                    Register = request.Register,
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description = "User not found." })
                };

                if (!string.IsNullOrEmpty(request.Register.Email) 
                    && EmailTest.IsValidEmail(request.Register.Email) 
                    && request.Register.Email.Trim() != user.Email)
                {
                    user.Email = request.Register.Email.Trim();
                    isChangeMail = true;

                    var identityResult = _signInManager.UserManager.UpdateAsync(user);
                    if (identityResult.Result.Succeeded)
                    {

                    }

                }
                

                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing change password for user {Login}.", result.Register.Login);
                result.IdentityResult = IdentityResult.Failed(new IdentityError { Description="Internal Error." }); 
            }
            return result;
            
        }

        #endregion

    }
}
