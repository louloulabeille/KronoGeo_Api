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
        private const int _maxWidth  = 2040;    // -- taille maximal pour l'image ce qui vaut un capeur de 12.5 megapixixels
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

                // 3. Utiliser SKCodec pour lire les métadonnées et l'orientation EXIF
                using var codec = SKCodec.Create(inputStream) ;
                
                if (codec == null) return null;

                // Récupérer l'orientation EXIF
                SKEncodedOrigin origin = codec.EncodedOrigin;

                inputStream.Position = 0; // -- mettre le flux au début pour le décodage
                using SKBitmap originalBitmap = SKBitmap.Decode(inputStream);
                if (originalBitmap is null) return null;

                // 4. CORRECTION DE L'ORIENTATION (Le cœur de la solution)
                SKBitmap orientedBitmap = FixOrientation(originalBitmap, origin);

                // Redimensionner selon si la taille dépasse
                SKBitmap resizeBitmap = orientedBitmap;

                if (orientedBitmap.Width > _maxWidth) // -- redimension de l'image
                {
                    int calculateHeight = (int)(orientedBitmap.Height * ((float)_maxWidth / orientedBitmap.Width));
                    resizeBitmap = orientedBitmap.Resize(new SKImageInfo(_maxWidth, calculateHeight), SKSamplingOptions.Default);
                }

                // -- compression ou non en jpeg
                using SKImage image = SKImage.FromBitmap(resizeBitmap);
                using SKData data = image.Encode(SKEncodedImageFormat.Jpeg, _tauxQualite);

                // Nettoyage si on a créé une nouvelle bitmap redimensionnée
                if (resizeBitmap != orientedBitmap) resizeBitmap.Dispose();
                if (orientedBitmap != originalBitmap) orientedBitmap.Dispose();

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

        #region private method 
        // Méthode d'aide pour appliquer la rotation/symétrie basée sur l'orientation EXIF
        /// <summary>
        /// IA method
        /// </summary>
        /// <param name="bitmap"></param>
        /// <param name="origin"></param>
        /// <returns></returns>
        private static SKBitmap FixOrientation(SKBitmap bitmap, SKEncodedOrigin origin)
        {
            // Si l'image est déjà "TopLeft" (standard), aucune correction n'est nécessaire
            if (origin == SKEncodedOrigin.TopLeft)
            {
                return bitmap; // On retourne le bitmap original sans le disposer
            }

            // Calculer les dimensions cibles (elles s'inversent pour une rotation de 90/270°)
            int width = bitmap.Width;
            int height = bitmap.Height;
            bool swapDimensions = (origin == SKEncodedOrigin.RightTop || origin == SKEncodedOrigin.LeftBottom ||
                                   origin == SKEncodedOrigin.LeftTop || origin == SKEncodedOrigin.RightBottom);

            if (swapDimensions)
            {
                width = bitmap.Height;
                height = bitmap.Width;
            }

            // Créer un nouveau bitmap pour le résultat
            SKBitmap rotatedBitmap = new (width, height);

            using SKCanvas canvas = new(rotatedBitmap);
            
            // Déplacer le point de rotation au centre
            canvas.Translate(width / 2f, height / 2f);

            // Appliquer la rotation/symétrie selon le tag EXIF
            switch (origin)
            {
                // Case 1 (TopLeft) est déjà géré au début
                case SKEncodedOrigin.TopRight: // Case 2
                    canvas.Scale(-1, 1); // Miroir horizontal
                    break;
                case SKEncodedOrigin.BottomRight: // Case 3
                    canvas.RotateDegrees(180); // Rotation 180°
                    break;
                case SKEncodedOrigin.BottomLeft: // Case 4
                    canvas.Scale(1, -1); // Miroir vertical
                    break;
                case SKEncodedOrigin.LeftTop: // Case 5 (Rotation 90° anti-horaire + Miroir H)
                    canvas.RotateDegrees(-90);
                    canvas.Scale(-1, 1);
                    break;
                case SKEncodedOrigin.RightTop: // Case 6 (Rotation 90° horaire)
                    canvas.RotateDegrees(90); // *** La rotation la plus fréquente ***
                    break;
                case SKEncodedOrigin.RightBottom: // Case 7 (Rotation 90° horaire + Miroir H)
                    canvas.RotateDegrees(90);
                    canvas.Scale(-1, 1);
                    break;
                case SKEncodedOrigin.LeftBottom: // Case 8 (Rotation 90° anti-horaire)
                    canvas.RotateDegrees(-90);
                    break;
            }

            // Dessiner l'image originale centrée sur le point de rotation
            //canvas.DrawBitmap(bitmap, -bitmap.Width / 2f, -bitmap.Height / 2f); // -- Obsolete
            var point = new SKPoint(-bitmap.Width / 2f, -bitmap.Height / 2f);
            canvas.DrawBitmap(bitmap, point, SKSamplingOptions.Default);
            
            return rotatedBitmap; // Retourne le nouveau bitmap corrigé
        }
        #endregion
    }
}
