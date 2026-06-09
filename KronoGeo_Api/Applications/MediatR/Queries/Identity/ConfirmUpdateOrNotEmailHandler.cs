using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Infrastructure.Service.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class ConfirmUpdateOrNotEmailHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<ConfirmUpdateOrNotEmailCommand, RegisterIdentity>
    {
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
                var identityResult = await _signInManager.UserManager.ChangeEmailAsync(user, decodeMail, decodeToken);
                result.IdentityResult = identityResult;
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
