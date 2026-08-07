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
        , IServiceCamera serviceCamera , IServiceSaveUser serviceSaveUser) 
        : IServiceSaveLocalisation
    {

        #region private readonly properties
        private readonly IServiceHttpKronoGeo _serviceHttpKronoGeo = serviceHttpKronoGeo;
        private readonly IServiceCamera _serviceCamera = serviceCamera;
        private readonly IServiceSaveUser _serviceSaveUser = serviceSaveUser;
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

                string token = await GetToken();
                LocalisationGroupDTO dTO = new() {
                    ApplicationUserId = localisations.ApplicationUserId,
                    Name = localisations.Name,
                    Date = localisations.Date,
                    Id = localisations.Id,
                    Localisations = localisations.Localisations?.Select(s =>
                    {
                        if (s is LocalisationPhoto photo)
                        {
                            return new LocalisationPhotoDTO()
                            {
                                Id = s.Id,
                                Latitude = s.Latitude,
                                Longitude = s.Longitude,
                                Accuracy = s.Accuracy,
                                Altitude = s.Altitude,
                                Course = s.Course,
                                OrderIndex = s.OrderIndex,
                                Speed = s.Speed,
                                Timestamp = s.Timestamp,
                                Name = photo.Name,
                                PathPhoto = photo.PathPhoto
                            };
                        }
                        else
                        {
                            return new LocalisationDTO()
                            {
                                Id = s.Id,
                                Latitude = s.Latitude,
                                Longitude = s.Longitude,
                                Accuracy = s.Accuracy,
                                Altitude = s.Altitude,
                                Course = s.Course,
                                OrderIndex = s.OrderIndex,
                                Speed = s.Speed,
                                Timestamp = s.Timestamp
                            };
                        }
                        
                    }
                    ).ToList()
                };
                var retour = await _serviceHttpKronoGeo.SaveGroupLocalisationsAsync(dTO, token);

                if (retour.IsSuccess)
                {
                    return true;
                }else
                {
                    return false;
                }
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
            string token = await GetToken();
            return await _serviceHttpKronoGeo.SavePhotoAsync(photo, token);
        }

        /// <summary>
        /// retourne le token de l'utilisateur
        /// </summary>
        /// <returns></returns>
        /// <exception cref="System.Exception"></exception>
        private async Task<string> GetToken()
        {
            var register = await _serviceSaveUser.GetRegister();
            if (register is null || string.IsNullOrEmpty(register.Token))
                throw new System.Exception("L'utilisateur n'est pas connecté.");

            return register.Token;
        }
        #endregion
    }
}
