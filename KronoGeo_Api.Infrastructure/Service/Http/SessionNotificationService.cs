using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Http
{
    /// <summary>
    /// class Handler qui est appelé quand la session est expirée
    /// quand le client http retourne Unauthorized
    /// pour savoir qu'elle action il doit faire
    /// </summary>
    public class SessionNotificationService
    {
        #region public properties event 
        public event Action? OnSessionExpired;
        #endregion

        #region public properties
        public Guid Guid = Guid.NewGuid();
        #endregion

        #region public method
        public void NotifySessionExpired()
        {
           /* if (OnSessionExpired is null)
                throw new NullReferenceException();*/
            OnSessionExpired?.Invoke();
        }
        #endregion
    }
}
