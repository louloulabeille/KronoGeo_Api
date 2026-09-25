using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Maui.Applications.Message;
using Microsoft.Maui.Controls.Maps;
using System;
using System.Collections.Generic;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;

namespace KronoGeo_Maui.Applications.Behaviors
{
    public partial class PinMessengerBehavior : Behavior<Map>
    {

        #region private properties
        private readonly Dictionary<string, Pin> _pinIndex = [];
        #endregion

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
                        var key = $"{message.Value.Pin.Label} | {message.Value.Pin.Address}";

                        if (message.Value.IsAdded) { 
                            bindable.Pins.Add(message.Value.Pin);
                            _pinIndex[key] = message.Value.Pin;
                        }
                        else
                        {
                            if( _pinIndex.TryGetValue(key, out var pin ))
                            {
                                bindable.Pins.Remove(pin);
                                _pinIndex.Remove(key);
                            }
                        }
                            
                    }
                    else
                    {
                        bindable.Pins.Clear();
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
            _pinIndex.Clear();
        }
    }
}
