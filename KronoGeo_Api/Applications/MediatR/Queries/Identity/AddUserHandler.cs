using KronoGeo_Api.Applications.Authentification;
using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class AddUserHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<AddUserCommand, RegisterIdentity>
    {

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
                    {
                        // - Assign Admin & User role to the first user
                        await _signInManager.UserManager.AddToRolesAsync(user, ["Admin", "User"]);
                    }
                    else // - Assign default role
                        await _signInManager.UserManager.AddToRoleAsync(user, "User");

                    // - token generation for the user after registration
                    registerIdentity.Register.Token =
                        await SecurityTokenGenerate.GenerateJwtToken(user, _keyBearer.Value, _signInManager.UserManager);
                }
                registerIdentity.Result = result;

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing login for user {Login}.", ex.Message);
                // - en cas d'erreur, on peut choisir de retourner un résultat spécifique ou de propager l'exception
                // pour le moment on retourne un résultat avec SignInResult null pour indiquer une erreur
                registerIdentity.SignInResult = null;
            }

            return registerIdentity;
        }
        #endregion
    }
}
