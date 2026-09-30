using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace KronoGeo_Blazor.Components.Api
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImagesBFFController(IMediator mediaR, ILogger<ImagesBFFController> logger) : Controller
    {
        #region private readonly properties
        private readonly ILogger<ImagesBFFController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion

        #region action 
        /// <summary>
        /// Action pôur modifier spécifiquement les points photos
        /// </summary>
        /// <param name="user">id de l'utilisateur</param>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost("UpdateImage/{user}")]
        public async Task<IActionResult> UpdateImage (string user, [FromBody] LocalisationPhotoDTO photo)
        {
            try {

                if ( !ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                var command = new UpdatePhotoCommand { IdUser = user, Photo = photo };
                var result = _mediaR.Send(command);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification de localisationPhotoDTO au niveau de l'Api BFF");
                return this.Problem("Error while updating localisation photo");
            }

        }

        #endregion

    }
}
