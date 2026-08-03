using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Maui.Applications.Message;
using System;
using System.Collections.Generic;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace KronoGeo_Maui.Applications.Behaviors
{
    public partial class PinMessengerBehavior : Behavior<Map>
    {
        /// <summary>
        /// ajoute un pin sur la carte
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);
            WeakReferenceMessenger.Default.Register<PinMapMessage>(this, (recipient, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (message.Value is not null)
                    {
                        bindable.Pins.Add(message.Value);
                    }
                });
            });
        }

        /// <summary>
        /// eviter les fuites mémoire en se désabonnant du message
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            // -- eviter les fuites mémoire en se désabonnant du message
            WeakReferenceMessenger.Default.Unregister<PinMapMessage>(this);
        }
    }
}
