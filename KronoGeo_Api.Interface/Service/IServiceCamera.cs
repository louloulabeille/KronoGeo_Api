using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceCamera
    {
        public Task<byte[]?> TakePhotoAsync();
    }
}
