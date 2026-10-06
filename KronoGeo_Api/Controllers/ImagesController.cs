using KronoGeo_Api.Applications.MediatR.Commands.Images;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace KronoGeo_Api.Controllers
{
    [Route("api/v1/[controller]")]
    [ApiController]
    public class ImagesController(ILogger<ImagesController> logger , IMediator mediaR) : Controller
    {
        #region private readonly properties
        private readonly ILogger<ImagesController> _logger = logger;
        private readonly IMediator _mediaR = mediaR;
        #endregion


        #region public action 
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
                if (!ModelState.IsValid)
                {
                    return this.BadRequest("Invalid model state.");
                }

                var command = new AddPhotoCommand() { FormFile = file };
                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'enregistrement des photos.");
                return this.Problem("Error while saving photos.");
            }
        }

        /// <summary>
        /// Méthod pour modifier la LocalisationPhoto
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        [HttpPost("UpdateImage")]
        public async Task<IActionResult> UpdateImage([FromBody] LocalisationPhotoDTO photo )
        {
            try
            {
                if( !ModelState.IsValid )
                {
                    return this.BadRequest("Invalid Model state.");
                }

                var command = new UpdatePhotoCommand() { Photo = photo };
                var result = await _mediaR.Send(command);

                return this.Ok(result);
            }catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification de localisationPhotoDTO ");
                return this.Problem("Error while updating localisation photo");
            }
        }

        /// <summary>
        /// Supprime la photo au niveau du serveur on garde le point de localisationPhoto
        /// en localisation au niveau de la base de données
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("DeleteImage/{id}")]
        public async Task<IActionResult> DeleteImage(int id)
        {
            try
            {
                var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
                if ( !ModelState.IsValid)
                {
                    return this.BadRequest("Invalid Model state.");
                }

                var command = new DeletePhotoCommand() { IdPhoto = id };
                var result = await _mediaR.Send(command);
                return this.Ok(result);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de la modification de localisationPhotoDTO ");
                return this.Problem("Error while Delete photo");
            }
        }
        #endregion

    }
}
