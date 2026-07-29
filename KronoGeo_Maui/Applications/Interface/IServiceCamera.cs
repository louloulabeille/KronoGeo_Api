using System;
using System.Collections.Generic;
using System.Text;
#if ANDROID
using Android.Content;    
#endif


namespace KronoGeo_Maui.Applications.Interface
{
    public interface IServiceCamera
    {
#if ANDROID
        public Context? Context { get; set; }
#endif

        public Task<byte[]?> TakePhotoAsync();
    }
}
