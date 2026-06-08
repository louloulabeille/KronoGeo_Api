using KronoGeo_Api.Applications.Email;
using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod;
using KronoGeo_Api.Infrastructure.Test;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class UpdateUserEmailHandler( ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager
        , IServiceSendMessage serviceSendMail
        , IOptions<UrlOptions> urlOptions)
        : UserIdentityHandler (logger, keyBearer, signInManager)
        , IRequestHandler<UpdateUserEmailCommand, RegisterIdentity>
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
        public async Task<RegisterIdentity> Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
        {
            RegisterIdentity result = new() { Register = request.Register };
            try
            {
                var user = await _signInManager.UserManager.FindByIdAsync(request.Register.Id) ??
                await _signInManager.UserManager.FindByNameAsync(request.Register.Login);

                if (user is null) return new RegisterIdentity()
                {
                    Register = request.Register,
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description = "User not found." })
                };

                // - modification de email
                if (!string.IsNullOrEmpty(request.Register.Email)
                    && EmailTest.IsValidEmail(request.Register.Email)
                    && request.Register.Email.Trim() != user.Email)
                {

                    string token = await _signInManager.UserManager.GenerateEmailConfirmationTokenAsync(user);
                    var identityResult = await _signInManager.UserManager.ChangeEmailAsync(user, request.Register.Email.Trim(), token);
                    if (identityResult.Succeeded)
                    {
                        string url = GenerateUrl.GenerationUrlAuthentification(_urlOptions, user, token);

                        // - Send confirmation email 
                        var MailAuto = new ApiMailIdentity(_serviceSendMail, _logger);
                        var resultEmail = MailAuto.SendEmail(user, new MessageChangeEmailCreator(_signInManager.UserManager, user, _urlOptions, url));
                    }
                    result.IdentityResult = identityResult;
                }
                else
                {
                    result.IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Email invalid" });
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
