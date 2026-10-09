using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog.Core;
using System.Security.Claims;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace KronoGeo_Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class GpsController (ILogger<AuthenticateController> logger,
        IMediator mediaR) : ControllerBase
    {
        #region private properties
        private readonly ILogger<AuthenticateController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion

        #region public action methods
        /// <summary>
        /// Retourne tous les groupes de localisations par UserId
        /// classé par Id group desc
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        // GET: api/v1/<GpsController>/GetAllGroup/{idUser}
        [HttpGet("GetAllGroup")]
        //public async Task<IActionResult> Get([FromQuery] string idUser)
        public async Task<IActionResult> Get()
        {
            var idUser = User.FindFirstValue("Id");
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for GetAllGroupLocalisations. idUser {idUser}", idUser);
                    return BadRequest("Invalid model state.");
                }

                if (string.IsNullOrEmpty(idUser))
                {
                    _logger.LogWarning("User Id is required for GetAllGroupLocalisations. idUser {idUser}", idUser);
                    return Unauthorized("User Id is required");
                }

                var command = new GetGroupLocalisationsCommand() { IdUser = idUser };
                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des groupes des localisations. idUser {idUser}", idUser);
                return this.Problem("Error search locations.");
            }
                    
        }

        /// <summary>
        /// retourne un group de localisation par id group
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        // GET api/v1/<GpsController>/5
        [HttpGet("GetLocalisationsById/{id}")]
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    _logger.LogWarning("Invalid model state for GetLocalisations. id {id}", id);
                    return BadRequest("Invalid model state.");
                }

                var command = new GetLocalisationsCommand() { IdLocalisationGroup = id };
                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi d'un groupe de localisation. id group localisation {id}", id);
                return this.Problem("Error search location");
            }
            
        }

        // POST api/v1/<GpsController>/Save
        [HttpPost("Save")]
        public async Task<IActionResult> SaveLocalisations([FromBody] LocalisationGroupDTO value)
        {
            try
            {
                if (!ModelState.IsValid || string.IsNullOrEmpty(value.ApplicationUserId))
                {
                    return BadRequest("Invalid model state.");
                }

                var command = new AddLocalisationsCommand() { LocalisationGroup = value };
                
                var result = await _mediaR.Send(command);

                return Ok(result);
            }
            catch (Exception ex)
            { 
                _logger.LogError(ex, "Erreur lors de la sauvegarde des localisations.");
                return this.Problem("Error while saving localisations.");
            }
        }

        
        // DELETE api/v1/<GpsController>/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id, [FromBody] UserIdDTO user)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return this.BadRequest("Invalid model state.");
                }
                var command = new DeleteLocalisationsCommand()
                {
                    IdLocalisationGroup = id,
                    IdUser = user.Id
                };

                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }catch(Exception ex)
            {
                _logger.LogError(ex,"Erreur lors de la suppression d'un trajet {id} par user {idUser}", id, user.Id);
                return this.Problem("Error while deleting Gps route.");
            }
        }

        [HttpPost("UpdateLocalisationGroup")]
        public async Task<IActionResult> UpdateLocalisationGroup([FromBody] LocalisationGroupDTO localisationGroup)
        {
            try
            {
                var idUser = User.FindFirstValue("Id");
                if (!ModelState.IsValid)
                {
                    return this.BadRequest("Invalid model state.");
                }

                if (string.IsNullOrEmpty(idUser))
                {
                    _logger.LogWarning("User Id is required for GetAllGroupLocalisations. idUser {idUser}", idUser);
                    return Unauthorized("User Id is required");
                }

                var command = new UpdateLocalisationGroupCommand()
                {
                    localisationGroup = localisationGroup
                };

                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur de l'enregistrement localisationgroup : {id} ", localisationGroup.Id);
                return this.Problem("Erreur lors de l'enregistrement.");
            }
        }
        #endregion
    }
}
