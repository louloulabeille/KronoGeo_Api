using Android.Hardware.Camera2;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Application.Camera
{
    internal class CameraCaptureCallback(Action<CaptureResult> onCompleted) : CameraCaptureSession.CaptureCallback
    {
        readonly Action<CaptureResult> _onCompleted = onCompleted;
        public override void OnCaptureCompleted(CameraCaptureSession session, CaptureRequest request, TotalCaptureResult result) => _onCompleted?.Invoke(result);
    }
}
