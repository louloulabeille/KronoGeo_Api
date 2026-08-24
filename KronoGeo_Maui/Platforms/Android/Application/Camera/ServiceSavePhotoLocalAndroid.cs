
using Android.Graphics;
using Android.Media;
using AndroidX.Core.Graphics;
using KronoGeo_Api.Interface.Service;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace KronoGeo_Maui.Platforms.Android.Application.Camera
{
    public class ServiceSavePhotoLocalAndroid : IServiceSavePhotoOsDirectory
    {
        /// <summary>
        /// Enregistre les photos dans le répertoire local
        /// Android
        /// </summary>
        /// <param name="streamPhoto"></param>
        /// <returns></returns>
        public async Task SavePhotoLocalAlbumAsync(System.IO.Stream streamPhoto, string namePhoto)
        {
            using System.IO.Stream stream = streamPhoto;
            using var memoryStream = new MemoryStream();
            stream.CopyTo(memoryStream);

            stream.Position = 0;
            memoryStream.Position = 0;
            var context = Platform.CurrentActivity;
            
            if (OperatingSystem.IsAndroidVersionAtLeast(29))
            {

                // -- préparation du type mime à insérer avec le nom de l'image
                global::Android.Content.ContentResolver? resolver = context?.ContentResolver;
                global::Android.Content.ContentValues contentValues = new();
                contentValues.Put(global::Android.Provider.MediaStore.IMediaColumns.DisplayName, namePhoto);
                contentValues.Put(global::Android.Provider.MediaStore.IMediaColumns.MimeType, "image/jpeg");
                contentValues.Put(global::Android.Provider.MediaStore.IMediaColumns.RelativePath, "DCIM/" + "image");

                // -- répertoire par défaut imges d'android
                var mediaUri = global::Android.Provider.MediaStore.Images.Media.ExternalContentUri;
                if (mediaUri is null) return;

                // -- insertion dans la base de gestion des images d'android chemin d'accès de la photo
                // -- et du type mines 
                global::Android.Net.Uri? imageUri = resolver?.Insert(mediaUri, contentValues);

                if (imageUri is null) return;

                // -- ouverture du flux pour l'enregistrement
                var streamOs = resolver?.OpenOutputStream(imageUri);
                global::Android.Graphics.BitmapFactory.Options options = new() 
                {
                    InJustDecodeBounds = true,
                };

                // -- creation du bitmap avec le flux Stream

                //var bitmap = global::Android.Graphics.BitmapFactory.DecodeStream(stream);
                var bitmap = OrientationMatrix(stream);

                var format = global::Android.Graphics.Bitmap.CompressFormat.Jpeg;
                if (format is null || streamOs is null || bitmap is null) return;

                // -- enregistrement de l'image dans son emplacement
                bitmap?.Compress(format, 100, streamOs);
                streamOs?.Flush();
                streamOs?.Close();
            }
            else
            {
                Java.IO.File? storagePath = global::Android.OS.Environment.GetExternalStoragePublicDirectory(global::Android.OS.Environment.DirectoryPictures);
                
                if (storagePath is null) return;

                string path = System.IO.Path.Combine(storagePath.ToString(), namePhoto);
                System.IO.File.WriteAllBytes(path, memoryStream.ToArray());
                var mediaScanIntent = new global::Android.Content.Intent(global::Android.Content.Intent.ActionMediaScannerScanFile);
                mediaScanIntent.SetData(global::Android.Net.Uri.FromFile(new Java.IO.File(path)));
                context?.SendBroadcast(mediaScanIntent);
            }
        }

        /// <summary>
        /// Retourne le bitmap ave la bonne orientation pour enregistrement
        /// </summary>
        /// <param name="stream"></param>
        /// <returns></returns>
        public Bitmap? OrientationMatrix(System.IO.Stream stream)
        {
            if (OperatingSystem.IsAndroidVersionAtLeast(24))
            {
                var mei = new ExifInterface(stream);
                int orientation = mei.GetAttributeInt(ExifInterface.TagOrientation, (int)Orientation.Normal);

                Matrix matrix = new ();
                switch (orientation)
                {
                    case (int)Orientation.Rotate90: matrix.PostRotate(90); break;
                    case (int)Orientation.Rotate180: matrix.PostRotate(180); break;
                    case (int)Orientation.Rotate270: matrix.PostRotate(270); break;
                }

                stream.Position = 0;
                var bitmap = global::Android.Graphics.BitmapFactory.DecodeStream(stream);

                if (bitmap is null) return null;

                Bitmap newBitmap = Bitmap.CreateBitmap(bitmap, 0, 0, bitmap.Width, bitmap.Height, matrix, true);

                return newBitmap;
            }

            return default;
        }

    }
}
