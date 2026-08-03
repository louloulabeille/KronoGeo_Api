using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Controls.Maps;
using KronoGeo_Maui.Applications.Message;

namespace KronoGeo_Maui.Applications.Behaviors
{
    public partial class PolyneMessengerBehavior : Behavior<Map>
    {
        /// <summary>
        /// Behavior qui permet d'ajouter des points à une polyline 
        /// sur la carte lorque la localisation change
        /// </summary>
        /// <param name="bindable"></param>
        protected override void OnAttachedTo(Map bindable)
        {
            base.OnAttachedTo(bindable);

            WeakReferenceMessenger.Default.Register<PolyneMapMessage>(this, (recipient, message) =>
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    if (bindable.MapElements.Count == 0)
                    {
                        var polyne = new Polyline()
                        {
                            StrokeColor = Colors.Blue,
                            StrokeWidth = 10,
                        };
                        if (message.Value is not null)
                        {
                            polyne.Geopath.Add(message.Value);
                        }
                        bindable.MapElements.Add(polyne);
                    }
                    else
                    {
                        var element = bindable.MapElements.FirstOrDefault() as Polyline;
                        if (message.Value is not null)
                        {
                            element?.Geopath.Add(message.Value);
                        }else
                        {
                            // -- si la localisation est null, on supprime la polyline
                            bindable.MapElements.Clear();
                        }
                    }
                });
            });
        }

        protected override void OnDetachingFrom(Map bindable)
        {
            base.OnDetachingFrom(bindable);
            // -- eviter les fuites mémoire en se désabonnant du message
            WeakReferenceMessenger.Default.Unregister<PolyneMapMessage>(this);
        }
    }
}
