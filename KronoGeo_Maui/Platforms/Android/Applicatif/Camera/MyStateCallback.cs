using Android.Hardware.Camera2;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Applicatif.Camera
{
    internal class MyStateCallback : CameraCaptureSession.StateCallback
    {
        public override void OnConfigured(CameraCaptureSession session)
        {
            // session prête
        }

        public override void OnConfigureFailed(CameraCaptureSession session)
        {
            // échec
        }
    }
}
