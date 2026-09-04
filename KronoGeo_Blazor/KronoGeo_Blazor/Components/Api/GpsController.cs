using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KronoGeo_Blazor.Components.Api
{
    [Microsoft.AspNetCore.Mvc.Route("api/v1/[controller]")]
    [ApiController]
    public class GpsController( IMediator mediaR, ILogger<GpsController> logger ) : Controller
    {
        #region private readonly properties
        private readonly IMediator _mediaR = mediaR;
        //private readonly IMemoryCache _memoryCache = memoryCache;
        private readonly ILogger<GpsController> _logger = logger;
        #endregion

        /// <summary>
        /// Retourne tous les groupes de localisations par UserId
        /// classé par Id group desc
        /// </summary>
        /// <param name="idUser"></param>
        /// <returns></returns>
        // GET: api/v1/<GpsController>/GetAllGroup/{idUser}
        [HttpGet("GetAllGroup/{idUser}")]
        public async Task<IActionResult> Get(string idUser)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                var result = await _mediaR.Send(new GroupLocationUserCommand() { UserId = idUser });

                return Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'envoi des groupes des localisations. idUser {idUser}", idUser);
                return this.Ok( new ResponseApiLocalisations() 
                { 
                    ApiStatus = EnumApiStatus.Problem, 
                    Message = "Erreur interne.",
                    LocalisationGroupDTO = null,
                    GroupsDTO = []
                });
            }
        }
    }
}
