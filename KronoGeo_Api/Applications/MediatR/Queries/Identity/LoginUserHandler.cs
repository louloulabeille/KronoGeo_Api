using KronoGeo_Api.Applications.Authentification;
using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    /// <summary>
    /// Handler for processing user login requests.
    /// It inherits from UserIdentityHandler to utilize common identity-related functionalities 
    /// and implements IRequestHandler to handle LoginUserCommand requests, 
    /// returning a RegisterIdentity result.
    /// </summary>
    /// <param name="logger"></param>
    /// <param name="keyBearer"></param>
    /// <param name="signInManager"></param>
    public class LoginUserHandler(ILogger<LoginUserHandler> logger,
        IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager) :
        UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<LoginUserCommand, RegisterIdentity>
    {
        /// <summary>
        /// Handles the user login process by validating the provided credentials
        /// and returning a RegisterIdentity object.
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RegisterIdentity> Handle(LoginUserCommand request, CancellationToken cancellationToken)
        {
            RegisterIdentity result = new() { Register = request.Register };

            try
            {
                // - recherche de l'utilisateur par login ou email
                var user = await _signInManager.UserManager.FindByNameAsync(result.Register.Login)??
                    await _signInManager.UserManager.FindByEmailAsync(result.Register.Email);

                // - si l'utilisateur existe et que le mot de passe est fourni, tenter de se connecter
                if (user is not null && !string.IsNullOrEmpty(result.Register.Password))
                {
                    var signInResult = await _signInManager.PasswordSignInAsync(user, result!.Register.Password, true, true);
                    result.SignInResult = signInResult;

                    if (signInResult.Succeeded)
                    {
                        // - génération du token JWT pour l'utilisateur connecté
                        result.Register.Token = 
                            await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer.Value, _signInManager.UserManager);
                    }
                 
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login for user {Login}.", result.Register.Login);
                // - en cas d'erreur, on peut choisir de retourner un résultat spécifique ou de propager l'exception
                // pour le moment on retourne un résultat avec SignInResult null pour indiquer une erreur
                result.SignInResult = null;
            }

            return result;
        }
    }
}
