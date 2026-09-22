using Android.OS;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Platforms.Android.Applicatif.OsManager
{
    /// <summary>
    /// classe qui retourne certains informations sur le matériel sous Android
    /// </summary>
    public static class DeviceInfos
    {
        public static string Manufacturer => Build.Manufacturer?.ToLowerInvariant() ?? string.Empty;
        public static string Model => Build.Model ?? string.Empty;
        public static int SdkVersion => (int)Build.VERSION.SdkInt;
    }
}
