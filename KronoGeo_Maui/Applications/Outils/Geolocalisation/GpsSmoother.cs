using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Outils.Geolocalisation
{
    /// <summary>
    /// classe de calcul d'acceptation d'un point Gps selon sa valeur de précision
    /// et lissage du point s'il est compris entre 8 et 30 - supprimer si > 30
    /// </summary>
    public class GpsSmoother
    {
        #region private properties
        private Location? _lastAcceptedLocation;
        #endregion

        #region public const properties
        // -- const accuracy acceptable 0 - 30 & 8 - 30 calcul de l'indice pour le lissage
        public const double MaxAcceptableAccuracy = 30.0;
        public const double ExcellentAccuracy = 8.0;
        #endregion

        #region public method
        public Location? AcceptableLocationCalcul(Location newLocation )
        {
            double trustFactor = 1.0; // -- facteur de confiance en %

            // -- si la location Accuracy is null alors on prend sans faire de calcul et on le retourne
            // -- a voir pour la suite 
            if (!newLocation.Accuracy.HasValue)
            {
                _lastAcceptedLocation = newLocation;
                return newLocation;
            }

            // > 30 on ignore
            if (newLocation.Accuracy > MaxAcceptableAccuracy) return null;

            // -- best precision pas besoin de calcul ou le premier point
            if (newLocation.Accuracy <= ExcellentAccuracy || _lastAcceptedLocation is null)
            {
                _lastAcceptedLocation = newLocation;
                return newLocation;
            }

            if (newLocation.Accuracy.Value > ExcellentAccuracy)
            {
                // Exemple : si accuracy = 19m, le calcul fera (30 - 19) / (30 - 8) = 11 / 22 = 0.5
                // On fera donc confiance à 50% à ce nouveau point, et à 50% à l'ancien.
                trustFactor = (MaxAcceptableAccuracy - newLocation.Accuracy.Value) / (MaxAcceptableAccuracy - ExcellentAccuracy);
            }

            // 5. Lissage par interpolation linéaire (Lerp)
            // On "tire" le nouveau point vers l'ancien en fonction de notre niveau de confiance
            double smoothedLat = Lerp(_lastAcceptedLocation.Latitude, newLocation.Latitude, trustFactor);
            double smoothedLon = Lerp(_lastAcceptedLocation.Longitude, newLocation.Longitude, trustFactor);

            // 6. Création du point lissé à renvoyer
            var smoothedLocation = new Location(smoothedLat, smoothedLon, newLocation.Timestamp)
            {
                Accuracy = newLocation.Accuracy,
                Altitude = newLocation.Altitude,
                Speed = newLocation.Speed,
                VerticalAccuracy = newLocation.VerticalAccuracy,
                Course = newLocation.Course,
            };

            // On sauvegarde ce point lissé comme référence pour le prochain calcul
            _lastAcceptedLocation = smoothedLocation;

            return smoothedLocation;
        }
        #endregion

        #region private method
        // Fonction mathématique d'interpolation (Linear Interpolation)
        private static double Lerp(double start, double end, double amount)
        {
            return start + (end - start) * amount;
        }
        #endregion
    }
}
