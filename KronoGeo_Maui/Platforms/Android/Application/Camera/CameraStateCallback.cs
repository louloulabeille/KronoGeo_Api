using Android.Hardware.Camera2;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Application.Camera
{
    internal class CameraStateCallback(Action<CameraDevice> opened, Action<CameraDevice> disconnected, Action<CameraDevice, CameraError> error) : CameraDevice.StateCallback
    {
        readonly Action<CameraDevice> _opened = opened;
        readonly Action<CameraDevice> _disconnected = disconnected;
        readonly Action<CameraDevice, CameraError> _error = error;

        public override void OnOpened(CameraDevice camera) => _opened?.Invoke(camera);
        public override void OnDisconnected(CameraDevice camera) => _disconnected?.Invoke(camera);
        public override void OnError(CameraDevice camera, CameraError error) => _error?.Invoke(camera, error);
    }
}
