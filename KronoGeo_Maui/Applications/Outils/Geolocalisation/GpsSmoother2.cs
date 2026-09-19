using KronoGeo_Api.Interface.AbstractClass;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Outils.Geolocalisation
{
    /// <summary>
    /// class à implementer en scoped pôur garder en mémoire le good last location
    /// </summary>
    public class GpsSmoother2 : BaseGpsSmoother
    {
        #region private properties
        private Location? _lastGoodPoint = default;
        private DateTimeOffset? _degradationStartTime = default;
        private static readonly TimeSpan MAX_DEGRADATION_DURATION = TimeSpan.FromMinutes(2);

        #endregion

        #region public properties new
        public new double MaxAcceptableAccuracy = 60.0; // -- max acceptable accuracy 60 mètres noramlement 30 
        #endregion

        #region public method abstract BaseGpsSmoother
        public override Location? AcceptableLocationCalcul(Location newLocation)
        {
            // -- si location is good on le retourne directemnt
            if (newLocation.Accuracy <= ExcellentAccuracy)
            {
                _degradationStartTime = null;
                _lastGoodPoint = newLocation;
                return newLocation;
            }

            _degradationStartTime ??= newLocation.Timestamp;

            // -- si location est très mauvais aucun traitement dessus
            if (newLocation.Accuracy > MaxAcceptableAccuracy)
                return null;

            var elapsed = newLocation.Timestamp - _degradationStartTime.Value;

            // - si au bout de 2 minutes de mesures dégradé on les injecte directement sans traitement
            if (elapsed > MAX_DEGRADATION_DURATION)
                return newLocation;

            if (_lastGoodPoint is null) return newLocation;

            return SmoothLocation(newLocation, _lastGoodPoint);

        }

        public override void Reset()
        {
            _lastGoodPoint = null;
            _degradationStartTime = null;
        }
        #endregion


        #region private method
        private static Location? SmoothLocation(Location newPoint, Location lastGoodPoint)
        {
            if (newPoint.Accuracy is null && lastGoodPoint.Accuracy is null) return null;

            double weightNew = (newPoint.Accuracy is > 0)? 1.0 / (newPoint.Accuracy.Value * newPoint.Accuracy.Value): 0;
            double weightOld = (lastGoodPoint.Accuracy is > 0) ? 1.0 / (lastGoodPoint.Accuracy.Value * lastGoodPoint.Accuracy.Value ): 0;
            double totalWeight = weightNew + weightOld;

            if (totalWeight == 0) return null;

            var latitude = (newPoint.Latitude * weightNew + lastGoodPoint.Latitude * weightOld) / totalWeight;
            var longitude = (newPoint.Longitude * weightNew + lastGoodPoint.Longitude * weightOld) / totalWeight;
            var accuracy = (double)Math.Sqrt(1.0 / totalWeight);

            return new(latitude, longitude)
            {
                Accuracy = accuracy,
                AltitudeReferenceSystem = newPoint.AltitudeReferenceSystem,
                Course = newPoint.Course,
                ReducedAccuracy = newPoint.ReducedAccuracy,
                Altitude = newPoint.Altitude,
                Speed = newPoint.Speed,
                Timestamp = newPoint.Timestamp,
                VerticalAccuracy = newPoint.VerticalAccuracy
            };
            
        }
        #endregion
    }
}
