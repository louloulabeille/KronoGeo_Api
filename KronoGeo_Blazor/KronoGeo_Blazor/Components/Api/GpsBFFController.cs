using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace KronoGeo_Blazor.Components.Api
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    [ApiController]
    public class GpsBFFController( IMediator mediaR, ILogger<GpsBFFController> logger ) : Controller
    {
        #region private readonly properties
        private readonly IMediator _mediaR = mediaR;
        //private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<GpsBFFController> _logger = logger;
        #endregion

        /// <summary>
        /// Retourne tous les groupes de localisations par UserId
        /// classé par Id group desc
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        // GET: api/v1/<GpsController>/GetAllGroup
        [HttpGet("GetAllGroup")]
        public async Task<IActionResult> Get()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }
                
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                var result = await _mediaR.Send(new GroupLocationUserCommand());

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des groupes des localisations. idUser {idUser}", userId);
                return this.Ok( new ResponseApiLocalisations() 
                { 
                    ApiStatus = EnumApiStatus.Problem, 
                    Message = "Erreur interne.",
                    LocalisationGroupDTO = null,
                    GroupsDTO = []
                });
            }
        }

        /// <summary>
        /// action Api qui retourne un groupe de localisations par IdGroup
        /// </summary>
        /// <param name="idGroup"></param>
        /// <returns></returns>
        [HttpGet("GetLocalisationsById/{idGroup}")]
        public async Task<IActionResult> GetGroupById(int idGroup)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }
                if (string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }
                var result = await _mediaR.Send(new GroupLocalisationsByIdCommand() { Id = idGroup });
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi du groupe des localisations. idUser {idUser}, idGroup {idGroup}", userId, idGroup);
                return this.Ok(new ResponseApiLocalisations()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Erreur interne.",
                    LocalisationGroupDTO = null,
                    GroupsDTO = []
                });
            }
        }

        /// <summary>
        /// Action Bff pour la sauvegarde de Localisation group
        /// </summary>
        /// <param name="localisationGroup"></param>
        /// <returns></returns>
        [HttpPost("UpdateLocalisationGroup")]
        public async Task<IActionResult> UpdateLocalisationGroup([FromBody] LocalisationGroupDTO localisationGroup)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }
                if (string.IsNullOrEmpty(userId) || userId != localisationGroup.ApplicationUserId )
                {
                    return Unauthorized("User is not authenticated.");
                }
                var result = await _mediaR.Send(new UpdateLocalisationGroupCommand() 
                    { LocalisationGroup = localisationGroup });
                return Ok(result);

            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la sauvegarde de la localisation group {id} : {message}", localisationGroup.Id, ex.Message);
                return this.Ok(new ResponseApiLocalisations
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = $"Erreur lors de la sauvegarde"
                });
            }
        }
    }
}
