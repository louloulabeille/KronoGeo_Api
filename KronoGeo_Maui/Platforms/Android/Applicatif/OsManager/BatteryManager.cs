using Android.Content;
using Android.OS;
using Application = Android.App.Application;
using Settings = Android.Provider.Settings;
using System;
using System.Collections.Generic;
using System.Text;
using KronoGeo_Api.Interface.Service;

namespace KronoGeo_Maui.Platforms.Android.Applicatif.OsManager
{
    public class BatteryManager : IServiceBattery
    {
        /// <summary>
        /// Retourne si le device est en économie d'énergie
        /// qui va désactiver le système de géolocalisation
        /// </summary>
        /// <returns></returns>
        public bool IsBatterySaver()
        {
            var powerManager = Application.Context.GetSystemService(Context.PowerService) as PowerManager;
            bool isBatterySaverOn = powerManager?.IsPowerSaveMode ?? false;

            return isBatterySaverOn;
        }

        /// <summary>
        /// Permet d'ouvrir la fenêtre de gestion de l'économie d'énergie
        /// </summary>
        /// <returns></returns>
        public void OpenWindowBatterySaver()
        {
            var intent = new Intent(Settings.ActionBatterySaverSettings);
            var activity = Platform.CurrentActivity;
            activity?.StartActivity(intent);
        }

    }

    /// <summary>
    /// class de broadcastReceiver pour les écoutes pendant la géolocalisation si l'utilisateur
    /// remet l'économie d'énergie
    /// </summary>
    //[BroadcastReceiver(Enabled = true, Exported = false)]
    public class PowerSaveModeReceiver : BroadcastReceiver
    {
        private readonly Action<bool> _onChanged;

        //public PowerSaveModeReceiver() { }

        public PowerSaveModeReceiver(Action<bool> onChanged)
        {
            _onChanged = onChanged;
        }

        public override void OnReceive(Context? context, Intent? intent)
        {
            if (context is null) return;
            var powerManager = context.GetSystemService(Context.PowerService) as PowerManager;
            bool isPowerSaveMode = powerManager?.IsPowerSaveMode ?? false;
            _onChanged?.Invoke(isPowerSaveMode);
        }
    }
}
