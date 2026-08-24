using Foundation;
using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.Text;
using UIKit;

namespace KronoGeo_Maui.Platforms.iOS.Application
{
    public class ServiceSavePhotoLocalIOS : IServiceSavePhotoOsDirectory
    {
        /// <summary>
        /// enregistre la photo dans le répertoire local des photos 
        /// pour IOS
        /// </summary>
        /// <param name="streamPhoto"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public async Task SavePhotoLocalAlbumAsync(Stream streamPhoto, string namePhoto)
        {
            // Convert Stream to byte[]
            byte[] imageData;
            using (MemoryStream ms = new ())
            {
                streamPhoto.CopyTo(ms);
                imageData = ms.ToArray();
            }

            // Convert byte[] to NSData
            var nsData = NSData.FromArray(imageData);

            // Load UIImage from NSData
            var image = UIImage.LoadFromData(nsData);

            // Save image to Photos Album
            image?.SaveToPhotosAlbum((img, error) =>
            {
                if (error != null)
                {
                    // Handle error
                    throw new Exception($"Error saving image: {error.LocalizedDescription}");
                }
            });
        }
    }
}
