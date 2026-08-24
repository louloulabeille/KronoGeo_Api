using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    /// <summary>
    /// interface pour enregistrement des images dans le répertoire 
    /// images selon Os utilisé
    /// </summary>
    public interface IServiceSavePhotoOsDirectory
    {
        public Task SavePhotoLocalAlbumAsync(Stream streamPhoto, string namePhoto);
    }
}
