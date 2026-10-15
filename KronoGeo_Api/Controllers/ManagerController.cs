using KronoGeo_Api.Applications.MediatR.Commands.Identity;
using KronoGeo_Api.Applications.MediatR.Commands.Manager;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
                return this.Ok(result);
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
        public async Task<IActionResult> UpdatePassword([FromBody] ProfilDTO profil)
        {

            try
            {
                var result = await _mediaR.Send(new UpdateUserPasswordCommand { Profil = profil });

                return this.Ok(result);  
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while updating password for user {id}.", profil.Login);
                return this.Problem("An error occurred while processing your request.");
            }
        }
        #endregion

        #region public action method get UserManager
        [HttpGet("GetProfil")]
        public async Task<IActionResult> GetProfil()
        {
            // -- récuépration de id dans la tokken
            var idUser = User.FindFirstValue("Id");
            try
            {
                var result = await _mediaR.Send(new ProfilUserCommand { IdUser = idUser ?? string.Empty });
                return this.Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "An error occurred while get profile for user {id}.", idUser);
                return this.Problem("An error occurred while processing your request profil.");
            }
        }
        #endregion
    }
}
