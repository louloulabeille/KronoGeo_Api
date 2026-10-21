using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Manager
{
    public class ProfilUserHandler(ILogger<ProfilUserHandler> logger
        , SignInManager<ApplicationUser> signInManager, IOptions<KeyBearer> keyBearer)
        : ManagerHandler(logger, signInManager, keyBearer)
        , IRequestHandler<ProfilUserCommand, ResponseApiManager>
    {
        public async Task<ResponseApiManager> Handle(ProfilUserCommand request, CancellationToken cancellationToken)
        {
            
            try
            {
                if (string.IsNullOrEmpty(request.IdUser))
                {
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = "Bad request." };
                }

                var user = await SignInManager.UserManager.FindByIdAsync(request.IdUser ?? string.Empty);
                
                if( user is null)
                {
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.NotFound, Message = "User not found" };
                }
                else
                {
                    var roles = await SignInManager.UserManager.GetRolesAsync(user);

                    return new ResponseApiManager
                    {
                        ApiStatus = EnumApiStatus.Success,
                        Profil = new ProfilDTO
                        {
                            Id = user?.Id,
                            Login = user?.UserName ?? string.Empty,
                            Email = user?.Email ?? string.Empty,
                            PhoneNumber = user?.PhoneNumber ?? string.Empty,
                            Roles = roles.ToList(),
                        },
                    };
                    
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "An error occurred while processing return user {id}.", request.IdUser);
                return new ResponseApiManager { ApiStatus = EnumApiStatus.Problem, Message = "An error occurred while processing return user" };
            }
            
        }
    }
}
