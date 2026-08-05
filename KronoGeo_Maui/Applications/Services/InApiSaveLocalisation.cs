using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class InApiSaveLocalisation(IServiceHttpKronoGeo serviceHttpKronoGeo
        , IServiceCamera serviceCamera) : IServiceSaveLocalisation
    {

        #region private readonly properties
        private readonly IServiceHttpKronoGeo _serviceHttpKronoGeo = serviceHttpKronoGeo;
        private readonly IServiceCamera _serviceCamera = serviceCamera;
        #endregion


        #region public method Interface IServiceSaveLocalisation
        /// <summary>
        /// sauvegarde les localisations dans l'API
        /// </summary>
        /// <param name="localisations"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<bool> SaveLocalisation(LocalisationGroup localisations, CancellationToken cancellationToken)
        {
            // -- envoi des photos
            var photos = localisations.Localisations?.OfType<LocalisationPhoto>().ToList();

            try
            {
                if (photos is not null || photos?.Count > 0)
                {
                    foreach (var photo in photos)
                    {
                        PhotoDTO photoDTO = new ()
                        {
                            Name = photo.Name,
                            PathPhoto = photo.PathPhoto
                        };
                        var result = await SavePhotoAsync(photoDTO);
                        if (result.IsSuccess)
                        {
                            // -- modification de localisationPhoto dans la liste pour modifier le path
                            localisations.Localisations?.OfType<LocalisationPhoto>()?
                                .FirstOrDefault(f=> f == photo)?.PathPhoto 
                                = result?.PhotoDTO?.PathPhoto;

                            // -- supprime l'image
                            _serviceCamera.DeletePhoto(photoDTO.PathComplet??string.Empty);
                        }
                    }
                }
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception($"Erreur lors de l'envoi des photos. {ex.Message}", ex);
            }
        }
        #endregion

        #region private method
        /// <summary>
        /// method pour enregister les photos au niveau de l'API
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        private async Task<ResponseApiImage> SavePhotoAsync (PhotoDTO photo)
        {
            return await _serviceHttpKronoGeo.SavePhotoAsync(photo);
        }
        #endregion
    }
}
