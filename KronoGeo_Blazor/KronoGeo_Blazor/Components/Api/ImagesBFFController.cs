using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

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
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost("UpdateImage")]
        public async Task<IActionResult> UpdateImage ([FromBody] LocalisationPhotoDTO photo)
        {

            try {

                if ( !ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                var command = new UpdatePhotoCommand { Photo = photo };
                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification de localisationPhotoDTO au niveau de l'Api BFF");
                return this.Problem("Error while updating localisation photo");
            }

        }

        /// <summary>
        /// Action pour supprimer spécifiquement les points photos
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage([FromRoute] int id)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                var command = new DeletePhotoCommand { Id = id };
                var result = await _mediaR.Send(command);
                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la suppression de localisationPhotoDTO au niveau de l'Api BFF");
                return this.Problem("Error while deleting localisation photo");
            }
        }

        #endregion

    }
}
