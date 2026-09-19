using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Interface
{
    public interface IServicePermissions
    {
        public Task<bool> GetLocalisationPermissionAsync();
        public Task<bool> GetPhotoPermissionAsync();
        public Task<bool> GetNotificationPermissionAsync();
        public void GestionBatterieAsync();
    }
}
