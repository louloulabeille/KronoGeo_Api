using System;
using System.Collections.Concurrent;
using System.Diagnostics;
using Microsoft.Maui.Controls;

namespace KronoGeo_Maui.Applications.Outils.Route
{
    internal static class RouteRegistrar
    {
        static readonly ConcurrentDictionary<string, byte> _registered = new();

        public static void RegisterOnce(string route, Type pageType)
        {
            if (string.IsNullOrWhiteSpace(route)) throw new ArgumentNullException(nameof(route));
            if (pageType == null) throw new ArgumentNullException(nameof(pageType));

            if (!_registered.TryAdd(route, 0))
                return; // déjà enregistré

            try
            {
                Routing.RegisterRoute(route, pageType);
            }
            catch (Exception ex)
            {
                // Retirer l'entrée pour permettre une nouvelle tentative ultérieure
                _registered.TryRemove(route, out _);
                Debug.WriteLine($"Route registration failed for '{route}': {ex}");
            }
        }

        public static void RegisterOnce(Type pageType)
        {
            if (pageType == null) throw new ArgumentNullException(nameof(pageType));
            var route = pageType.FullName!;
            RegisterOnce(route, pageType);
        }
    }
}
