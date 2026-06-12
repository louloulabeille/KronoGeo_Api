using KronoGeo_Api.Applications.Authentification;
using KronoGeo_Api.Applications.Email;
using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Infrastructure.Service.Email.MessageFactoryMethod;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class AddUserHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager
        , IServiceSendMessage serviceSendMail
        , IOptions<UrlOptions> urlOptions)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<AddUserCommand, RegisterIdentity>
    {
        #region private properties
        private readonly IServiceSendMessage _serviceSendMail = serviceSendMail;
        private readonly IOptions<UrlOptions> _urlOptions = urlOptions;
        #endregion


        #region method interface
        public async Task<RegisterIdentity> Handle(AddUserCommand request, CancellationToken cancellationToken)
        {
            RegisterIdentity registerIdentity = new() { 
                Register = request.Register
            };
            var user = new IdentityUser
            {
                UserName = string.IsNullOrEmpty(registerIdentity.Register.Login) ? registerIdentity.Register.Email : registerIdentity.Register.Login,
                Email = registerIdentity.Register.Email,
                PhoneNumber = registerIdentity.Register.PhoneNumber
            };
            try
            {
                var result = await _signInManager.UserManager.CreateAsync(user, registerIdentity.Register.Password);
                if (result.Succeeded)
                {
                    // -  for the first account is Admin and the others are User
                    if (_signInManager.UserManager.Users.Count() == 1)
                        // - Assign Admin & User role to the first user
                        await _signInManager.UserManager.AddToRolesAsync(user, ["Admin", "User"]);
                    else // - Assign default role
                        await _signInManager.UserManager.AddToRoleAsync(user, "User");

                    // - token generation for the user after registration
                    registerIdentity.Register.Token =
                        await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer.Value, _signInManager.UserManager);
                    registerIdentity.Register.Id = user.Id; // - va servir pour la suppression du compte
                    registerIdentity.IdentityResult = result;
                    registerIdentity.Register.Password = string.Empty;

                    // - Send confirmation email 
                    var MailAuto = new ApiMailIdentity(_serviceSendMail, _logger);
                    var resultEmail = MailAuto.SendEmail(user, new MessageAuthentificationCreator(_signInManager.UserManager, user, _urlOptions));

                    // - il faudra modifier toutes cette partie pour gérer la possilité de renvoyer le mail
                    // - 
                    if (resultEmail.Status == EmailResultStatus.Failure)
                    {
                        _logger.LogError("{message} - Error send mail for {userName}", resultEmail.Message, user.UserName);
                    }
                }
                
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login for user {Login}.", ex.Message);
                // - en cas d'erreur, on peut choisir de retourner un résultat spécifique ou de propager l'exception
                // pour le moment on retourne un résultat avec SignInResult failed pour indiquer une erreur
                registerIdentity.SignInResult = SignInResult.Failed;
            }

            return registerIdentity;
        }
        #endregion

        #region private method
        /*private MessageResult SendEmailConfirmation(IdentityUser user)
        {
            var message = MessageCourriel.Message(new MessageAuthentificationCreator( _signInManager.UserManager, user, _urlOptions));
            if (user.Email is null)
            {
               _logger.LogError("User {UserName} has no email address. Cannot send confirmation email.", user.UserName);
                return new MessageResult() 
                {
                    To = string.Empty,
                    Message = message,
                    Status = EmailResultStatus.Failure,
                    Exception = new ArgumentNullException($"{user.UserName} has no email address. Cannot send confirmation email.") 
                };
            }

            var result = _serviceSendMail.Send(user.Email, "Confirmation de votre adresse email", message);
            if (result.Status == EmailResultStatus.Failure)
            {
                _logger.LogError("Failed to send confirmation email to {Email}: {Message}", user.Email, result.Message);
            }
            return result;
        }*/
        #endregion

    }
}
