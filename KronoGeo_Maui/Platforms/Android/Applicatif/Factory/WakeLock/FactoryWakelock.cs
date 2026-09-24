using Android.OS;
using Android.Systems;
using Android.Util;
using KronoGeo_Api.Models.Infrastructure.Options;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Applicatif.Factory.WakeLock
{
    public class FactoryWakelock( IOptions<AndroidOSListBattery> options)
    {
        #region private readonly properties
        private readonly IOptions<AndroidOSListBattery> _options = options;
        #endregion

        #region public properties
        /// <summary>
        /// retourne le WakeLock selon Os utilisé et selon Os en black listé
        /// pour leur gestion agressive de la batterie
        /// </summary>
        /// <param name="powerManager"></param>
        /// <param name="Os"></param>
        /// <returns></returns>
        public PowerManager.WakeLock? GetWakeLock(PowerManager powerManager, string Os)
        {
            var flags = DetermineWakeLockFlags(Os);

            return powerManager.NewWakeLock(flags, "GeoAndroidService:BackgroundTrackingLock");
        }

        #endregion

        #region private method
        private WakeLockFlags DetermineWakeLockFlags (string Os)
        {
            if (_options.Value.ListOs.Any(s => s == Os))
            {
                Log.Debug("GeoAndroidService", "WakeLockFlags : WakeLockFlags.ScreenDim | WakeLockFlags.OnAfterRelease");
                if (OperatingSystem.IsAndroidVersionAtLeast(28))
                    return WakeLockFlags.ScreenDim | WakeLockFlags.OnAfterRelease;
            }
            Log.Debug("GeoAndroidService", "WakeLockFlags : WakeLockFlags.Partial");
            return WakeLockFlags.Partial;
        }
        #endregion
    }
}
