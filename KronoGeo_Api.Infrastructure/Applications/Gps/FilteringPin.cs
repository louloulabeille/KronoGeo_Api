using KronoGeo_Api.Models.RegionBounds;
using System;
using System.Collections.Generic;
using System.Text;

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


    /// <summary>
    /// Système de Filtrage par distance 
    /// & pixel carte pour la mise en place de cluster de Pin
    /// </summary>
    public class FilteringPin
    {

        #region const properties
        internal const double R = 6378137;
        #endregion

        #region public method static 

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
        /// Formule de Haversine pour le calcul de la distance orthodromique 
        /// ( calcul distance du + court chemin sur une sphère )
        /// Calcul entre 2 coordonnées GPS
        ///  -- solution Claude IA
        /// </summary>
        /// <param name="lat1"></param>
        /// <param name="lon1"></param>
        /// <param name="lat2"></param>
        /// <param name="lon2"></param>
        /// <returns></returns>
        public static double HaversineMeters(double lat1, double lon1, double lat2, double lon2)
        {
            const double R = 6371000; // rayon moyen de la Terre en m
            static double ToRad(double deg) => deg * Math.PI / 180;

            double dLat = ToRad(lat2 - lat1);
            double dLon = ToRad(lon2 - lon1);

            double a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2)
                     + Math.Cos(ToRad(lat1)) * Math.Cos(ToRad(lat2))
                     * Math.Sin(dLon / 2) * Math.Sin(dLon / 2);

            double c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
            return R * c;
        }

        /// <summary>
        /// Retourne les clusters selon la liste des Pins ajoutée dans la methode
        /// IA - Claude
        /// </summary>
        /// <param name="pins"></param>
        /// <param name="minDistanceMeters"></param>
        /// <returns></returns>
        public static List<ClusterPin> ClusterPins(List<Pin> pins, double minDistanceMeters)
        {
            var clusters = new List<ClusterPin>();

            foreach (var p in pins)
            {
                // cluster existant le plus proche, à moins de minDistanceMeters
                ClusterPin? nearest = null;
                double best = double.MaxValue;

                // -- vérification du cluster le plus proche pour le pin si existant 
                foreach (var c in clusters)
                {
                    double d = 
                        HaversineMeters(c.CentralCoordonate!.Latitude, c.CentralCoordonate!.Longitude, p.PinCoordonnee.Latitude, p.PinCoordonnee.Longitude);
                    // -- vérification pour avoir le meilleur cluster pour le Pin
                    if (d < minDistanceMeters && d < best)
                    {
                        best = d;
                        nearest = c;
                    }
                }

                // -- création du cluster ave le Pin
                if (nearest is null)
                {
                    var c = new ClusterPin { CentralCoordonate = 
                        new() { Latitude = p.PinCoordonnee.Latitude, Longitude = p.PinCoordonnee.Longitude }
                    };
                    c.Pins.Add(p);
                    clusters.Add(c);
                }
                else
                { // -- ajout dans la liste des pins du cluster
                    nearest.Pins.Add(p);
                    int n = nearest.Count;                       // recalcul incrémental du centre
                    nearest.CentralCoordonate?.Latitude += (p.PinCoordonnee.Latitude - nearest.CentralCoordonate.Latitude) / n;
                    nearest.CentralCoordonate?.Longitude += (p.PinCoordonnee.Longitude - nearest.CentralCoordonate.Longitude) / n;
                }
            }
            return clusters;
        }

        /// <summary>
        /// Claude Ai retourne les clusterPin selon pixel et cercle autour par rapport à la Map
        /// pour éviter les chevauchements sur la map des Pins ou 
        /// du moins les éviter au maximun
        /// </summary>
        /// <param name="pins"></param>
        /// <param name="mercPerPx">mètres Mercator par pixel (calcul du zoom)</param>
        /// <param name="minDistancePx">Deux pins sont regroupés si leurs centres sont à moins de minDistancePx (en pixel default 40)</param>
        /// <returns></returns>
        public static List<ClusterPin> ClusterByScreenDistance(
            IEnumerable<Pin> pins, double mercPerPx, double minDistancePx = 40)
        {
            var clusters = new List<Work>();

            // 1) passe gloutonne : ordre stable (par Id) pour que les clusters ne « sautent » pas
            foreach (var p in pins.OrderBy(p => p.OrderIndex))
            {
                var (mx, my) = ToMercator(p.PinCoordonnee.Latitude, p.PinCoordonnee.Longitude);
                double x = mx / mercPerPx, y = my / mercPerPx;

                // -- le plus proche
                Work? nearest = null;
                double best = minDistancePx;
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
                        if (d >= minDistancePx) continue;

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
                    CentralCoordonate = new()
                    {
                        Latitude = w.Pins.Average(p => p.PinCoordonnee.Latitude),
                        Longitude = w.Pins.Average(p => p.PinCoordonnee.Longitude)
                    }

                };
                c.Pins.AddRange(w.Pins);
                return c;
            }).ToList();
        }
        #endregion
    }
}
