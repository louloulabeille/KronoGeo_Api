using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog.Core;

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
        [HttpGet("GetAllGroup/{idUser}")]
        //public async Task<IActionResult> Get([FromQuery] string idUser)
        public async Task<IActionResult> Get(string idUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
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
        [HttpGet("{id}")]
        //public async Task<IActionResult> Get([FromQuery] int id)
        public async Task<IActionResult> Get(int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                var command = new GetLocalisationsCommand() { Id = id };
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

        /// <summary>
        /// enregistrement des images avant enregistrements des points Gps
        /// dans un répertoire temporaire
        /// faire un traitement de ce repertoire pour supprimer les fichiers de + de 24h
        /// </summary>
        /// <param name="file"></param>
        /// <returns></returns>
        // POST api/v1/<GpsController>/SaveImage
        [HttpPost("SaveImage")]
        public async Task<IActionResult> SaveImage(IFormFile file)
        {
            try
            {
                if( !ModelState.IsValid)
                {
                    return this.BadRequest("Invalid model state.");
                }

                var command = new AddPhotoCommand() { FormFile = file };
                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'enregistrement des photos.");
                return this.Problem("Error while saving photos.");
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
        #endregion
    }
}
