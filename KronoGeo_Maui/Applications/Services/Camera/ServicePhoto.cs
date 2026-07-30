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
                FileResult? photo = await MediaPicker.Default.CapturePhotoAsync();

                if (photo != null)
                {
                    // save the file into local storage
                    string localFilePath = Path.Combine(FileSystem.CacheDirectory, photo.FileName);

                    try
                    {
                        using Stream sourceStream = await photo.OpenReadAsync();
                        using FileStream localFileStream = File.OpenWrite(localFilePath);

                        // copie de la photo en local
                        await sourceStream.CopyToAsync(localFileStream);

                        return new PhotoDTO()
                        {
                            Name = photo.FileName,
                            PathPhoto = FileSystem.CacheDirectory
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
        #endregion
    }
}
