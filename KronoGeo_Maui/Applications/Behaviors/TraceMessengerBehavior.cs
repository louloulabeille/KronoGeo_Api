using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Maui.Applications.Message;
using System;
using System.Collections.Generic;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Controls.Maps;



#if ANDROID
using Android.Util;
#endif


namespace KronoGeo_Maui.Applications.Behaviors
{
    public class TraceMessengerBehavior : Behavior<Map>
    {
        #region private properties 
        private Polyline? _loaded;
        #endregion

        /// <summary>
        /// Method qui écoute le Messenger -> envoi de donnée 
        /// qui va tracer les points Gps
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
            // On s'abonne au message
            WeakReferenceMessenger.Default.Register<TraceMapMessage>(this, (recipient, message) =>
            {
                // On s'assure que l'appel UI se fait bien sur le thread principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
#if ANDROID
                    Log.Debug("GeoAndroidService", $"Polyne tracé ajout du tracé au niveau de la map");
                    Log.Debug("GeoAndroidService", $"--------------------------------------------------");
#endif
                    if ( _loaded is null )
                    {
                        // -- ajout du polyne à la map
                        _loaded = new Polyline { StrokeColor = Colors.Gray, StrokeWidth = 6 };
                    } 
                    
                    if ( message.Value is null && bindable.MapElements.Contains(_loaded) )
                    {
                        bindable.MapElements.Remove(_loaded);
                    }
                    else if( message.Value != null )
                    {
                        // -- ajout des points de location 
                        foreach (var location in message.Value) _loaded.Geopath.Add(location);
                        bindable.MapElements.Insert(0, _loaded);
                    }

                });
            });
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            // Très important : on se désabonne pour éviter les fuites mémoire
            WeakReferenceMessenger.Default.Unregister<TraceMapMessage>(this);
        }
    }
}
