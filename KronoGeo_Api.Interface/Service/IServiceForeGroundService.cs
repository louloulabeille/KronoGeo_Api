using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceForegroundService
    {
        public void StartService();
        public void PauseService();
        public void StopPauseService();
        public void StopService();
    }
}
