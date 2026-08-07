using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Api.Models.Model.DTO;
using System.Diagnostics;

namespace KronoGeo_Maui.Applications.Services.Camera
{
    /// <summary>
    /// service de prise de photo
    /// </summary>
    public class ServicePhoto : IServiceCamera
    {
        #region private readonly properties

        #endregion

        #region public method interface IServiceCamera
        public async Task<PhotoDTO?> TakePhotoAsync()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                // -- options de compression pour la prise des photos
                MediaPickerOptions options = new () {
                    CompressionQuality = 50
                };
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync(options);

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                    try
                    {
                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);

                        // copie de la photo en local
                        await sourceStream.CopyToAsync(localFileStream);

                        return new PhotoDTO()
                        {
                            Name = photo.FileName,
                            PathPhoto = FileSystem.AppDataDirectory,
                        };
                    }
                    catch(Exception ex)
                    {
                        Trace.TraceError(ex.Message);
                        return null;
                    }

                }
            }
            return null;
        }

        /// <summary>
        /// supprime toutes les photos enregistrer en local avant utilisation
        /// </summary>
        public void DeletePhotos()
        {
            // Récupère tous les chemins des fichiers se terminant par .jpg dans le cache
            string[] cachedPhotos = Directory.GetFiles(FileSystem.AppDataDirectory, "*.jpg");

            foreach (string photoPath in cachedPhotos)
            {
                File.Delete(photoPath);
            }
        }

        /// <summary>
        /// supprime un fichier chemin complet
        /// </summary>
        /// <param name="photoPath"></param>
        /// <returns></returns>
        public bool DeletePhoto(string photoPath)
        {
            if (File.Exists(photoPath))
            {
                File.Delete(photoPath); 
                return true;
            }
            return false;
        }

        #endregion
    }
}
