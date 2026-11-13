using CommunityToolkit.Mvvm.Messaging;
using System;
using System.Collections.Generic;
using System.Text;
using Map = Microsoft.Maui.Controls.Maps.Map;
using Microsoft.Maui.Controls.Maps;
using KronoGeo_Maui.Applications.Message;
#if ANDROID
using Android.Util;
#endif

namespace KronoGeo_Maui.Applications.Behaviors
{
    public partial class PolyneMessengerBehavior : Behavior<Map>
    {

        #region private properties
        private Polyline? _live;
        #endregion


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
#if ANDROID
                    Log.Debug("GeoAndroidService", $"Polyne Map Latitude : {message?.Value?.Latitude}");
                    Log.Debug("GeoAndroidService", $"--------------------------------------------------");
#endif
                    // -- création du polyline pour le tracé en court
                    if( _live is null)
                    {
                        _live = new Polyline()
                        {
                            StrokeColor = Colors.OrangeRed,
                            StrokeWidth = 8,
                        };
                        bindable.MapElements.Add(_live);
                        //bindable.MapElements.Insert(1, _live);
                    }

                    if ( message?.Value is not null )
                    {
                        _live.Geopath.Add(message.Value);
                    }
                    else
                    { // -- supprime tous les éléments en Map
                        bindable.MapElements.Clear();
                    }

                    /*if (bindable.MapElements.Count == 0)
                    {
                        if (message?.Value is not null)
                        {
                            polyne.Geopath.Add(message.Value);
                        }
                        // -- insert du polyne
                        // -- dans le bindable.MapElements à l'index 1 pour qu'il soit au dessus des autres éléments
                        // -- en 0 ce sera le tracé s'il existe
                        bindable.MapElements.Insert(1, polyne);
                        //bindable.MapElements.Add(polyne);
                    }
                    else
                    {
                        // -- on récupère la polyline existante à l'index 1 tracé en cours 
                        var element = bindable.MapElements.Count > 0 ? bindable.MapElements[1] as Polyline : null;
                        if (message?.Value is not null && element is not null)
                        { // -- si la localisation n'est pas null, on ajoute le point à la polyline
                            element?.Geopath.Add(message.Value);
                        }else if( message?.Value is null )
                        {
                            // -- si la localisation est null, on supprime la polyline
                            bindable.MapElements.Clear();
                        }
                    }*/
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
