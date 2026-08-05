using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Maui.Applications.Message;
using Microsoft.Maui.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace KronoGeo_Maui.Applications.Behaviors
{
    public class MapMessengerBehavior : Behavior<Map>
    {
        /// <summary>
        /// Behavior qui permet de recentrer la carte lorsqu'un message
        /// RecenterMapMessage est reçu via le CommunityToolkit.Mvvm.Messaging
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);

            // On s'abonne au message
            WeakReferenceMessenger.Default.Register<RecenterMapMessage>(this, (recipient, message) =>
            {
                // On s'assure que l'appel UI se fait bien sur le thread principal
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    bindable.MoveToRegion(
                        MapSpan.FromCenterAndRadius(message.Value, Distance.FromMeters(150))
                        //MapSpan.FromCenterAndRadius(message.Value,Distance.FromMeters(0))
                    );
                });
            });
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);

            // Très important : on se désabonne pour éviter les fuites mémoire
            WeakReferenceMessenger.Default.Unregister<RecenterMapMessage>(this);
        }
    }
}
