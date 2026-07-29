using Android.Media;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Application.Camera
{
    // Listener pour ImageReader
    /// <summary>
    /// class appelée quand une image est disponible dans ImageReader,
    /// elle lit les octets JPEG et les retourne via le TaskCompletionSource
    /// </summary>
    internal class ImageReaderListener : Java.Lang.Object, ImageReader.IOnImageAvailableListener
    {
        readonly TaskCompletionSource<byte[]> _tcs;
        public ImageReaderListener(TaskCompletionSource<byte[]> tcs) => _tcs = tcs;
        public void OnImageAvailable(ImageReader? reader)
        {
            if (reader is null) return;
            using var image = reader.AcquireLatestImage();

            if (image == null) return;

            var plane = image.GetPlanes()?[0];
            var buffer = plane?.Buffer;
            var bytes = new byte[buffer?.Remaining() ?? 0];
            buffer?.Get(bytes);

            if (!_tcs.Task.IsCompleted) _tcs.SetResult(bytes);

        }
    }
}
