using Android.Hardware.Camera2;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Application.Camera
{
    internal class CameraCaptureStateCallback(Action<CameraCaptureSession> configured, Action<CameraCaptureSession> failed) : CameraCaptureSession.StateCallback
    {
        readonly Action<CameraCaptureSession> _configured = configured;
        readonly Action<CameraCaptureSession> _failed = failed;
        public override void OnConfigured(CameraCaptureSession session) => _configured?.Invoke(session);
        public override void OnConfigureFailed(CameraCaptureSession session) => _failed?.Invoke(session);
    }
}
