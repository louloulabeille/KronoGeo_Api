using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.Model.DTO;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Email;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Identity
{
    public class UpdateUserPasswordHandler(ILogger<AddUserHandler> logger
        , IOptions<KeyBearer> keyBearer, SignInManager<IdentityUser> signInManager) 
        : UserIdentityHandler(logger, keyBearer, signInManager)
        , IRequestHandler<UpdateUserPasswordCommand, RegisterIdentity>
    {
        /// <summary>
        /// update le mot de passe - en vérifiant le nouveau comme l'ancien 
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<RegisterIdentity> Handle(UpdateUserPasswordCommand request, CancellationToken cancellationToken)
        {
            var result = new RegisterIdentity { Register = request.Register };

            if (string.IsNullOrEmpty(request.Register.Password))
                result.IdentityResult = IdentityResult.Failed(new IdentityError() { Description = "Password failed." });

            


            return result;
        }
    }
}
