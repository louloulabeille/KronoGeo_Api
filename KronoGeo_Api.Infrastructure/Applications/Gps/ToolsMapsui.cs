using KronoGeo_Api.Models;
using KronoGeo_Api.Models.RegionBounds;
using Mapsui;
using Mapsui.Layers;
using Mapsui.Projections;
using Mapsui.Styles;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using static Microsoft.Maui.ApplicationModel.Permissions;

namespace KronoGeo_Api.Infrastructure.Applications.Gps
{
    /// <summary>
    /// class qui va aider pour la création des clusterPin en prennant 
    /// en cac
    /// </summary>
    internal class Work
    {
        public double X, Y;                 // centre en pixels
        public List<Pin> Pins = new();
    }

    public static class ToolsMapSui
    {
        #region const properties
        internal const double R = 6378137;
        #endregion

        /// <summary>
        /// Transforme une localisation en position Mercator sur une carte plane
        /// </summary>
        /// <param name="lat"></param>
        /// <param name="lon"></param>
        /// <returns></returns>
        internal static (double x, double y) ToMercator(double lat, double lon)
        {
            lat = Math.Clamp(lat, -85.05, 85.05);
            return (R * lon * Math.PI / 180,
                    R * Math.Log(Math.Tan(Math.PI / 4 + lat * Math.PI / 360)));
        }

        /// <summary>
        /// Claude Ai retourne les clusterPin selon pixel et cercle autour par rapport à la Map
        /// pour éviter les chevauchements sur la map des Pins ou 
        /// du moins les éviter au maximun
        /// </summary>
        /// <param name="pins"></param>
        /// <param name="mercPerPx"></param>
        /// <param name="radiusPx"></param>
        /// <returns></returns>
        public static List<ClusterPin> ClusterByScreenDistance(
            IEnumerable<Pin> pins, double mercPerPx, double radiusPx = 40)
        {
            var clusters = new List<Work>();

            // 1) passe gloutonne : ordre stable (par Id) pour que les clusters ne « sautent » pas
            foreach (var p in pins.OrderBy(p => p.OrderIndex))
            {
                var (mx, my) = ToMercator(p.PinCoordonnee.Latitude, p.PinCoordonnee.Longitude);
                double x = mx / mercPerPx, y = my / mercPerPx;

                // -- le plus proche
                Work? nearest = null;
                double best = radiusPx;
                foreach (var c in clusters)
                {
                    double d = Math.Sqrt((c.X - x) * (c.X - x) + (c.Y - y) * (c.Y - y));
                    if (d < best) { best = d; nearest = c; }
                }

                if (nearest is null)
                    clusters.Add(new Work { X = x, Y = y, Pins = { p } });
                else
                {
                    int n = nearest.Pins.Count;
                    nearest.X = (nearest.X * n + x) / (n + 1);
                    nearest.Y = (nearest.Y * n + y) / (n + 1);
                    nearest.Pins.Add(p);
                }
            }

            // 2) passe de fusion : tant que deux clusters se chevauchent, on les fusionne
            bool merged;
            do
            {
                merged = false;
                for (int i = 0; i < clusters.Count && !merged; i++)
                    for (int j = i + 1; j < clusters.Count; j++)
                    {
                        var a = clusters[i]; var b = clusters[j];
                        double d = Math.Sqrt((a.X - b.X) * (a.X - b.X) + (a.Y - b.Y) * (a.Y - b.Y));
                        if (d >= radiusPx) continue;

                        int na = a.Pins.Count, nb = b.Pins.Count;
                        a.X = (a.X * na + b.X * nb) / (na + nb);
                        a.Y = (a.Y * na + b.Y * nb) / (na + nb);
                        a.Pins.AddRange(b.Pins);
                        clusters.RemoveAt(j);
                        merged = true;
                        break;
                    }
            } while (merged);

            // 3) conversion en ClusterPin (position = moyenne des coordonnées GPS des membres)
            return clusters.Select(w =>
            {
                var c = new ClusterPin
                {
                    CentralCoordonate = new() { 
                        Latitude = w.Pins.Average(p => p.PinCoordonnee.Latitude),
                        Longitude = w.Pins.Average(p => p.PinCoordonnee.Longitude)
                    }
                    
                };
                c.Pins.AddRange(w.Pins);
                return c;
            }).ToList();
        }

        /// <summary>
        /// Affiche les points layer pour les photos
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        public static MemoryLayer? AddPointImage(IEnumerable<Localisation> localisations, out Dictionary<IFeature,LocalisationPhoto> featureImageMap)
        {
            featureImageMap = [];

            var photos = localisations.OrderBy(x => x.OrderIndex).OfType<LocalisationPhoto>().ToList();

            if (photos is null || photos.Count == 0) return default;
            var features = new List<Mapsui.IFeature>();

            foreach (var photo in photos)
            {

                if (string.IsNullOrEmpty(photo.PathPhoto)) continue;

                // -- transformation des points longitude et latitude en points mercator
                var coordonate = SphericalMercator.FromLonLat(photo.Longitude, photo.Latitude);
                var feature = new PointFeature(coordonate);

                feature.Styles.Add(ImgPin());

                // -- ajout dans le dictionnaire pour retrouver les photos par rapport 
                // -- à leur feature
                featureImageMap.Add(feature, photo);

                // -- ajout dans la liste des features pour l'intégrer dans le layer
                features.Add(feature);
            }

            var memoryLayer = new MemoryLayer
            {
                Name = "Photo",
                Features = features
            };

            //_photoLayer = memoryLayer;

            return memoryLayer;

        }


        /// <summary>
        /// Affiche les points layer sur la map
        /// en retournant un MemoryLayer à ajouter au MapControl de Mapsui
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        public static MemoryLayer? AddPointImage(List<ClusterPin> clusterPins, out Dictionary<IFeature, ClusterPin> featureClusterPin)
        {
            featureClusterPin = [];

            // -- vérification si des enregistrements dans la liste
            if ( clusterPins.Count == 0) return default;
            // -- retour du feature qui retourné dans le MemoryLayer
            var features = new List<Mapsui.IFeature>();

            foreach (var clusterPin in clusterPins)
            {
                //if (string.IsNullOrEmpty(pin.Path)) continue;
                if (clusterPin.CentralCoordonate is null) continue;

                // -- transformation des points longitude et latitude en points mercator
                var coordonate = SphericalMercator.FromLonLat(clusterPin.CentralCoordonate.Longitude, clusterPin.CentralCoordonate.Latitude);
                var feature = new PointFeature(coordonate);

                feature.Styles.Add( DefaultPin() );
                // -- ajout dans le dictionnaire pour retrouver les photos par rapport 
                // -- à leur feature
                featureClusterPin.Add(feature, clusterPin);

                // -- ajout dans la liste des features pour l'intégrer dans le layer
                features.Add(feature);
            }

            var memoryLayer = new MemoryLayer
            {
                Name = "Pin",
                Features = features
            };

            return memoryLayer;
        }

        /// <summary>
        /// Retourne le style Pin pour qu'il soit afficher sur la Map
        /// </summary>
        /// <returns></returns>
        private static ImageStyle DefaultPin()
        {
            // -- currentColor => rouge #E53935
            var svg = $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"#E53935\" class=\"bi bi-geo-alt\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M12.166 8.94c-.524 1.062-1.234 2.12-1.96 3.07A32 32 0 0 1 8 14.58a32 32 0 0 1-2.206-2.57c-.726-.95-1.436-2.008-1.96-3.07C3.304 7.867 3 6.862 3 6a5 5 0 0 1 10 0c0 .862-.305 1.867-.834 2.94M8 16s6-5.686 6-10A6 6 0 0 0 2 6c0 4.314 6 10 6 10\"/>\r\n  <path d=\"M8 8a2 2 0 1 1 0-4 2 2 0 0 1 0 4m0 1a3 3 0 1 0 0-6 3 3 0 0 0 0 6\"/>\r\n</svg>";
            var imageStyle = new ImageStyle
            {

                Image = new Image { Source = $"svg-content://{svg}" },
                SymbolScale = 1,
            };

            return imageStyle;
        }

        /// <summary>
        /// Retourne le style image pour qu'il soit afficher
        /// </summary>
        /// <returns></returns>
        private static ImageStyle ImgPin()
        {
            // -- currentColor
            var color = "#6D28D9";
            var svg = $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"{color}\" class=\"bi bi-camera2\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M5 8c0-1.657 2.343-3 4-3V4a4 4 0 0 0-4 4\"/>\r\n  <path d=\"M12.318 3h2.015C15.253 3 16 3.746 16 4.667v6.666c0 .92-.746 1.667-1.667 1.667h-2.015A5.97 5.97 0 0 1 9 14a5.97 5.97 0 0 1-3.318-1H1.667C.747 13 0 12.254 0 11.333V4.667C0 3.747.746 3 1.667 3H2a1 1 0 0 1 1-1h1a1 1 0 0 1 1 1h.682A5.97 5.97 0 0 1 9 2c1.227 0 2.367.368 3.318 1M2 4.5a.5.5 0 1 0-1 0 .5.5 0 0 0 1 0M14 8A5 5 0 1 0 4 8a5 5 0 0 0 10 0\"/>\r\n</svg>";
            var imageStyle = new ImageStyle
            {
                Image = new Image { Source = $"svg-content://{svg}" },
                SymbolScale = 1,
            };

            return imageStyle;
        }

    }



}
