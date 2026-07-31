using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Api.Models.Model.DTO;

namespace KronoGeo_Maui.Applications.Interface
{
    public interface IServiceCamera
    {
        public Task<PhotoDTO?> TakePhotoAsync();
        public void DeletePhotos();
    }
}
