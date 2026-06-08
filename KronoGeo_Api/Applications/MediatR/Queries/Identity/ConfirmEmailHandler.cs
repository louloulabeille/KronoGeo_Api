using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Interface.Service;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Options;
using Microsoft.Win32;
using System.Text;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class ConfirmEmailHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<ConfirmEmailCommand, RegisterIdentity>
    {
        public async Task<RegisterIdentity> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            string id = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Id));
            string token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(request.Token));
            var user = await _signInManager.UserManager.FindByIdAsync(id);;

            if (user is null)
            {
                var register = new RegisterDTO() { Id = id , Login = string.Empty, Password = string.Empty };
                var identityResult = IdentityResult.Failed(new IdentityError { Description = "User not found." });

                return new RegisterIdentity() { Register = register, IdentityResult = identityResult};
            }
            var result = await _signInManager.UserManager.ConfirmEmailAsync(user, token);

            return new RegisterIdentity() { 
                Register = new RegisterDTO()
                {
                    Id = user.Id, 
                    Login = user.UserName??string.Empty,
                } ,
                IdentityResult = result 
            }; ;
        }
    }
}
