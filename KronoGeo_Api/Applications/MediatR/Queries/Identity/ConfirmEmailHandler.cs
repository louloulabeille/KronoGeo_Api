using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Interface.Service;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.Win32;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class ConfirmEmailHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager)
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<ConfirmEmailCommand, RegisterIdentity>
    {
        public async Task<RegisterIdentity> Handle(ConfirmEmailCommand request, CancellationToken cancellationToken)
        {
            
            var user = await _signInManager.UserManager.FindByIdAsync(request.Id);;

            if (user is null)
            {
                var register = new RegisterDTO() { Id = request.Id , Login = string.Empty, Password = string.Empty };
                var identityResult = IdentityResult.Failed(new IdentityError { Description = "User not found." });

                return new RegisterIdentity() { Register = register, Result = identityResult};
            }
            var result = await _signInManager.UserManager.ConfirmEmailAsync(user, request.Token);

            return new RegisterIdentity() { 
                Register = new RegisterDTO()
                {
                    Id = user.Id, 
                    Login = user.UserName??user.Email??string.Empty,
                } , 
                Result = result 
            }; ;
        }
    }
}
