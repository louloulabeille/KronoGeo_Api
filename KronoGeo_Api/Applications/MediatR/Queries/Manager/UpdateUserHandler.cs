using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace KronoGeo_Api.Applications.MediatR.Queries.Manager
{
    public class UpdateUserHandler(ILogger<UpdateUserHandler> logger
        , SignInManager<ApplicationUser> signInManager)
        : ManagerHandler(logger, signInManager)
        , IRequestHandler<UpdateUserCommand, ResponseApiManager>
    {
        /// <summary>
        /// modifie juste le login de l'utilisateur
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseApiManager> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (string.IsNullOrEmpty(request.IdUser) && string.IsNullOrEmpty(request.Profil.Email)
                    && string.IsNullOrEmpty(request.Profil.Login))
                {
                    return new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = "Bad request." };
                }

                // -- recherche de l'utilisateur
                var user = await  _signInManager.UserManager.FindByIdAsync(request.IdUser)??
                    await _signInManager.UserManager.FindByEmailAsync(request.Profil.Email??string.Empty)??
                    await _signInManager.UserManager.FindByNameAsync(request.Profil.Login??string.Empty);

                ResponseApiManager response = new();

                if (user is null)
                {  
                    response.ApiStatus = EnumApiStatus.NotFound;  
                    return response; 
                }

                // -- mise a jour du numéro de téléphone
                if( user.PhoneNumber?.Trim() != request.Profil.PhoneNumber?.Trim())
                {
                    var resultPhone = await _signInManager.UserManager.SetPhoneNumberAsync(user, request.Profil.PhoneNumber);
                    if( !resultPhone.Succeeded )
                    {
                        response.ApiStatus = EnumApiStatus.BadRequest;
                        response.Message = string.Join(",", resultPhone.Errors.Select(e => e.Description));
                        return response;
                    }
                }

                // -- mise a jour de l'username
                if (user.UserName?.Trim() != request.Profil.Login?.Trim())
                {
                    // -- mise a jour de l'username
                    var resultIdentity =  await _signInManager.UserManager.SetUserNameAsync(user, request.Profil.Login);
                    if( !resultIdentity.Succeeded )
                    {
                        response.ApiStatus = EnumApiStatus.BadRequest;
                        response.Message = string.Join(",", resultIdentity.Errors.Select(e=>e.Description));
                        return response;
                    }
                    
                    // -- mise a jour de la sécurité stamp qui prend en compte le login ou username
                    await _signInManager.UserManager.UpdateSecurityStampAsync(user);

                }

                response.Message = "L'utilisateur a été mis à jour.";
                response.Profil = request.Profil;

                return response;

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while processing return user {id}.", request.IdUser);
                return new ResponseApiManager { ApiStatus = EnumApiStatus.Problem, Message = "An error occurred while processing return user" };
            }
        }

    }
}
