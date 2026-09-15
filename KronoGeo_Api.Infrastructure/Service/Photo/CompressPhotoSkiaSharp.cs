using KronoGeo_Api.Interface.Service;
using Microsoft.Maui.Storage;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Photo
{
    public class CompressPhotoSkiaSharp : IServiceCompressPhoto
    {
        #region private const properties
        private const int _maxWidth  = 4080;    // -- taille maximal pour l'image ce qui vaut un capeur de 12.5 megapixixels
        private const int _tauxQualite = 100;   // -- Qualité de compression(0 à 100)
        #endregion


        /// <summary>
        /// method de compression des photos utilisation de SkiaSharp
        /// </summary>
        /// <param name="photo"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<byte[]?> GetCompressPhoto(FileResult photo)
        {
            try
            {
                // -- ouvrir le flux photo
                using Stream inputStream = await photo.OpenReadAsync();
                using SKBitmap originalBitmap = SKBitmap.Decode(inputStream);

                // Redimensionner selon si la taille dépasse
                SKBitmap resizeBitmap = originalBitmap;

                if (originalBitmap.Width > _maxWidth) // -- redimension de l'image
                {
                    int calculateHeight = (int)(originalBitmap.Height * ((float)_maxWidth / originalBitmap.Width));
                    resizeBitmap = originalBitmap.Resize(new SKImageInfo(_maxWidth, calculateHeight), SKSamplingOptions.Default);
                }

                // -- compression ou non en jpeg
                using SKImage image = SKImage.FromBitmap(resizeBitmap);
                using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, _tauxQualite);

                // Nettoyage si on a créé une nouvelle bitmap redimensionnée
                if (resizeBitmap != originalBitmap)
                    resizeBitmap.Dispose();

                // 4. Obtenir le tableau de bytes de l'image compressée
                byte[] compressedBytes = data.ToArray();

                return compressedBytes;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur dans la compression de la photo : \n {ex.Message}");
                return null;
            }
            

        }
    }
}
