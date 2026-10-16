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
        /// <summary>
        /// supression de son propre compte
        /// </summary>
        /// <returns></returns>
        [HttpDelete("DeleteUser")]
        // - suppression de son compte utilisateur de l'API
        public async Task<IActionResult> DeleteUser()
        {
            var id = User.FindFirstValue("Id");
            try
            {
                var result = await _mediaR.Send(new DeleteUserCommand() { Id = id ?? string.Empty });
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while deleting user {id}.", id);
                return this.Problem("An error occurred while processing your request.");
            }
        }

        /// <summary>
        /// action de suppression d'un compte par un admin ou manager
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteUser/{id}")]
        [Authorize(Roles = "Admin,Manager")]
        // - suppression de son compte utilisateur de l'API
        public async Task<IActionResult> DeleteUser(string id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = ModelState.ToString() });
                }

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
        public async Task<IActionResult> UpdatePassword([FromBody] ProfilDTO profil)
        {
            try
            {
                var idUser = User.FindFirstValue("Id");

                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = ModelState.ToString() });
                }

                if (idUser != profil.Id)
                    return Unauthorized();

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
        /// <summary>
        /// retourne le profil de l'utilisateur
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        /// Retourne tous les Profils
        /// bon pour commencer mais pour la suite voir ci dessous pour une recherche
        /// plus précise
        /// </summary>
        /// <returns></returns>
        [HttpGet("GetAllProfil")]
        [Authorize(Roles = "Admin,Manager")]
        public async Task<IActionResult> GetAllProfil()
        {
            // -- récupération de id dans la tokken
            // -- pour la vérification si l'utilisateur a les droits
            var idUser = User.FindFirstValue("Id");
            try
            {
                var result = await _mediaR.Send(new ProfilAllUsersCommand { IdUser = idUser ?? string.Empty });
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while get all profiles for users {id}.", idUser);
                return this.Problem("An error occurred while processing your request all profils.");
            }
        }

        //[HttpGet("GetProfil/{search}")]
        //[Authorize(Roles = "Admin,Manager")]
        //public async Task<IActionResult> GetProfil([FromRoute]string search)
        //{

        //}
        #endregion

        #region public action method update user
        /// <summary>
        /// Action pour modifier son profil
        /// </summary>
        /// <param name="profil"></param>
        /// <returns></returns>
        [HttpPost("UpdateUser")]
        [Authorize(Roles ="User")]
        public async Task<IActionResult> UpdateUser([FromBody] ProfilDTO profil )
        {
            // -- récupération de Id
            var idUser = User.FindFirstValue("Id");
            try
            {
                // -- validation du model state peut servir si on le désactive 
                // -- c'est possible
                if (!ModelState.IsValid)
                {
                    return BadRequest(new ResponseApiManager { ApiStatus = EnumApiStatus.BadRequest, Message = ModelState.ToString() });
                }

                if( idUser != profil.Id)
                {
                    return Unauthorized("Profil unauthorized");
                }

                // -- appel de MediaR  
                var result = await _mediaR.Send(new UpdateUserCommand { 
                    IdUser = idUser ?? string.Empty,
                    Profil = profil
                });

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while update  profile for users {id}.", idUser);
                return this.Problem("An error occurred while processing your request update your profil.");
            }
        }

        #endregion
    }
}
