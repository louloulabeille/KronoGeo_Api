using KronoGeo_Api.Applications.Authentification;
using KronoGeo_Api.Applications.Email;
using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class ConfirmUpdateOrNotEmailHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager
        , IOptions<UrlOptions> urlOptions, IServiceSendMessage serviceSendMail)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<ConfirmUpdateOrNotEmailCommand, RegisterIdentity>
    {

        #region private properties
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        private readonly IServiceSendMessage _serviceSendMail = serviceSendMail;
        #endregion

        /// <summary>
        /// Method de modification de mail - réceptionne le token de modification de mail pour valider la nouvelle adresse
        /// quand cela est fait - il envoie un mail de récupération vers l'ancienne adresse mail pour récupérer l'ancien mail
        /// au cas ou. Durée du token 24 heures à tester.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<RegisterIdentity> Handle(ConfirmUpdateOrNotEmailCommand request, CancellationToken cancellationToken)
        {
            var decodeId = GenerateUrl.DecodingMessage(request.Id);
            var decodeMail = GenerateUrl.DecodingMessage(request.Email);
            var decodeToken = GenerateUrl.DecodingMessage(request.Token);

            var user = await _signInManager.UserManager.FindByIdAsync(decodeId);
            var result = new RegisterIdentity(){Register = new RegisterDTO() { 
                Login = user?.UserName ?? string.Empty,
                Email = decodeMail,
                Id = decodeId,
            }};

            if (user is null)
            {
                result.IdentityResult = IdentityResult.Failed(new IdentityError { Description = "The user is unknown." });
                return result;
            }

            try
            {
                if ( decodeMail != user.Email )
                {
                    string oldMail = user.Email ?? string.Empty;

                    // - changement du mail avec la nouvelle adresse
                    var identityResult = await _signInManager.UserManager.ChangeEmailAsync(user, decodeMail, decodeToken);
                    result.IdentityResult = identityResult;

                    if (identityResult.Succeeded)
                    { // - modifie le token & fait l'envoi vers l'ancien compte du mail de récupération 
                        result.Register.Token = await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer.Value, _signInManager.UserManager);
                        if ( request.Recup && !string.IsNullOrEmpty(oldMail)) // - envoi du mail de récupêrationq
                        {
                            // - génération du token avec la nouvelle adresse mail
                            string tokenRecupt = await _signInManager.UserManager.GenerateChangeEmailTokenAsync(user, oldMail);
                            string urlRecupAccount = GenerateUrl.GenerationUrlRecupAccount(_urlOptions, user, oldMail, tokenRecupt);

                            // - Send confirmation email 
                            var mailAuto = new ApiMailIdentity(_serviceSendMail, _logger);
                            var recuptMail = mailAuto.SendEmail(oldMail, new MessageRecupOldEmailCreator(_signInManager.UserManager, user, _urlOptions, urlRecupAccount), "Mail de récupération de l'ancienne adresse mail.");
                            if (recuptMail.Status == EmailResultStatus.Failure)
                            {
                                _logger.LogError("An error occurred while processing send email recovery  {user}", user.UserName);
                                result.SignInResult = SignInResult.Failed;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing change email for user {login}.", user.UserName );
                result.IdentityResult = IdentityResult.Failed( new IdentityError { Description = "Error internal." });
            }

            return result;
        }
    }
}
