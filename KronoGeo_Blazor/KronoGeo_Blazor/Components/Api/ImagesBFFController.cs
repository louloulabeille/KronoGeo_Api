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
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

                if ( !ModelState.IsValid)
                {
                    return BadRequest("Invalid model state.");
                }

                if(string.IsNullOrEmpty(userId))
                {
                    return Unauthorized("User is not authenticated.");
                }

                var command = new UpdatePhotoCommand { IdUser = userId, Photo = photo };
                var result = await _mediaR.Send(command);

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
