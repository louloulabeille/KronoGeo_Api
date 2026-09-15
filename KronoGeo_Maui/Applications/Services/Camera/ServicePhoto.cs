#if ANDROID
using Android.Util;
#endif
using CommunityToolkit.Maui.Media;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace KronoGeo_Maui.Applications.Services.Camera
{
    /// <summary>
    /// service de prise de photo
    /// </summary>
    public class ServicePhoto(IServiceSavePhotoOsDirectory savePhotoOsDirectory
        , IServiceCompressPhoto compressPhoto ) : IServiceCamera
    {
        #region private readonly properties
        private readonly IServiceSavePhotoOsDirectory _savePhotoOsDirectory = savePhotoOsDirectory;
        private readonly IServiceCompressPhoto _compressPhoto = compressPhoto;
        #endregion

        #region public method interface IServiceCamera
        /// <summary>
        /// methode de prise de photo et d'enregistrement
        /// </summary>
        /// <returns></returns>
        public async Task<PhotoDTO?> TakePhotoAsync()
        {
            if (MediaPicker.Default.IsCaptureSupported)
            {
                // -- options de compression pour la prise des photos qui se fait avant la prise
                // -- dans prendre en compte la taille de l'image
                /*MediaPickerOptions options = new () {
                    CompressionQuality = 50
                };*/
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    
                    try
                    {
                        // save the file into local storage
                        string localFilePath = Path.Combine(FileSystem.AppDataDirectory, photo.FileName);
                        // -- stream de la photo 
                        using System.IO.Stream sourceStream = await photo.OpenReadAsync();
                        
                        // -- compression de la photo photo
                        var photoCompress = await _compressPhoto.GetCompressPhoto(photo);

                        if (photoCompress is null)
                        {
                            using FileStream localFileStream = File.OpenWrite(localFilePath);
                            // copie de la photo en local au niveau de l'applicatif
                            await sourceStream.CopyToAsync(localFileStream);
                            
                        }
                        else
                        {
                            await File.WriteAllBytesAsync(localFilePath, photoCompress);
                        }

                        // copie dans le répertoire des images selon OS
                        string filename = photo.FileName;
                        await _savePhotoOsDirectory.SavePhotoLocalAlbumAsync(sourceStream, filename);

                        return new PhotoDTO()
                        {
                            Name = photo.FileName,
                            PathPhoto = FileSystem.AppDataDirectory,
                        };

                    }
                    catch (Exception ex)
                    {
                        Trace.TraceError(ex.Message);
#if ANDROID
                        Log.Error("GeoAndroidService", ex.Message);
#endif
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
