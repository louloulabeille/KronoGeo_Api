using Microsoft.Maui.Storage;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceCompressPhoto
    {
        public Task<byte[]?> GetCompressPhoto(FileResult photo); 
    }
}
