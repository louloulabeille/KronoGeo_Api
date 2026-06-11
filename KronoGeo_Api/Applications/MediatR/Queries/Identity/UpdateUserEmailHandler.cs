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
    public class UpdateUserEmailHandler( ILogger<UpdateUserEmailHandler> logger
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
        /// method qui envoi un mail de confirmation pour le nouveau mail avant mise a jour
        /// et envoi aussi le mail de récuperation de compte sur l'ancienne adresse mail avant récupération
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
                    
                    
                    string token = await _signInManager.UserManager.GenerateChangeEmailTokenAsync(user, request.Register.Email.Trim());
                    string urlNewMail = GenerateUrl.GenerationUrlEmailUpdate(_urlOptions, user, request.Register.Email.Trim(), token);

                    // - email de recuperation 
                    var mailAuto = new ApiMailIdentity(_serviceSendMail, _logger);
                
                    var resultEmail = mailAuto.SendEmail(request.Register.Email, new MessageChangeEmailCreator(_signInManager.UserManager, user, _urlOptions, urlNewMail),"Mail de validation de changement de votre adresse mail.");
                    if (resultEmail.Status == EmailResultStatus.Success) { 
                        //_logger.LogDebug("Url {url} d'authentificatrion de modification de mail pour le compte : {login} ", urlNewMail, user.UserName);
                        result.IdentityResult = IdentityResult.Success;
                        return result;
                    }

                    result.IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Internal error or email not found." });
                    _logger.LogError("An error occurred while processing land email for user {Login}. old mail :{old} && new mail : {new}", user.UserName, user.Email, request.Register.Email);
                    
                }
                else
                {
                    result.IdentityResult = IdentityResult.Failed(new IdentityError { Description = "Email invalid" });
                    _logger.LogError("New Email invalid {new} for {user}", request.Register.Email, user.UserName);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing change email for user {Login}.", result.Register.Login);
                result.IdentityResult = IdentityResult.Failed(new IdentityError { Description="Internal Error." }); 
            }

            return result;
        }

        #endregion

    }
}
