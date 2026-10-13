using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KronoGeo_Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ManagerController (ILogger<AuthenticateController> logger
        , IMediator mediaR)
        : ControllerBase
    {

        #region private properties
        private readonly ILogger<AuthenticateController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion

        #region public action method Delete
        [HttpDelete("{id}")]
        [Authorize(Roles = "Admin,User,Manager")]
        // - suppression de son compte utilisateur de l'API
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                var result = await _mediaR.Send(new DeleteUserCommand() { Id = id });
                if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.Success, Message = "Deleted successfully." });
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.BadRequest, Message = result.IdentityResult.Errors.ToString() });

                _logger.LogError("An error occurred while deleting user {id}. pas de IdentityResult", id);
                return this.Problem("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user {id}.", id);
                return this.Problem("An error occurred while processing your request.");
            }
        }
        #endregion

        #region public action method Password // - mettre aussi dans cette région initialisation password 
        /// <summary>
        /// action de modification du mot de passe du compte utilisateur tout en verifiant que son mot de passe
        /// actuel est ok
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        [HttpPost("UpdatePassword")]
        [Authorize(Roles = "Admin,User,Manager")]
        public async Task<IActionResult> UpdatePassword([FromBody] RegisterDTO register)
        {

            try
            {
                var result = await _mediaR.Send(new UpdateUserPasswordCommand { Register = register });

                if (result.IdentityResult is not null && result.IdentityResult.Succeeded)
                {
                    return this.Ok(new ResponseApiAuthenticate { ApiStatus = EnumApiStatus.Success, Message = "Update password successfully." });
                }

                if (result.IdentityResult is not null)
                    return this.BadRequest(result.IdentityResult.Errors);

                _logger.LogError("An error occurred while updating password for user {user}. pas de IdentityResult", register.Login);
                return this.Problem("An error occurred while processing your request.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating password for user {id}.", register.Login);
                return this.Problem("An error occurred while processing your request.");
            }
        }

        #endregion

    }
}
