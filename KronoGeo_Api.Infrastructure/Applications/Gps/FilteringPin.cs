using KronoGeo_Api.Models.RegionBounds;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Applications.Gps
{
    /// <summary>
    /// Système de Filtrage par distance 
    /// & pixel carte pour la mise en place de cluster de Pin
    /// </summary>
    public class FilteringPin
    {
        #region public method static 

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
        #endregion
    }
}
