using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Infrastructure.Service.Email;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Options;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Text;
using System.Threading.Channels;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class ConfirmEmailHandler(ILogger<ConfirmEmailHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<ConfirmEmailCommand, RegisterIdentity>
    {
        public async Task<RegisterIdentity> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            try
            {
                string id = GenerateUrl.DecodingMessage(request.Id);
                string token = GenerateUrl.DecodingMessage(request.Token);

                var user = await _signInManager.UserManager.FindByIdAsync(id);

                if (user is null)
                {
                    var register = new RegisterDTO() { Id = id, Login = string.Empty, Password = string.Empty };
                    var identityResult = IdentityResult.Failed(new IdentityError { Description = "User not found." });

                    return new RegisterIdentity() { Register = register, IdentityResult = identityResult };
                }
                var result = await _signInManager.UserManager.ConfirmEmailAsync(user, token);

                return new RegisterIdentity()
                {
                    Register = new RegisterDTO()
                    {
                        Id = user.Id,
                        Login = user.UserName ?? string.Empty,
                    },
                    IdentityResult = result
                };
            }catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing change email for user {error}.", ex.Message);

                return new RegisterIdentity()
                {
                    Register = new RegisterDTO()
                    {
                        Id = request.Id,
                        Login = string.Empty,
                    },
                    IdentityResult = IdentityResult.Failed(new IdentityError { Description="Internal Error." })
                };
            }
        }
    }
}
