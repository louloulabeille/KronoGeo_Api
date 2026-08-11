using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Telemetry
{
    public class ServiceTelemetry (IServiceSaveParametrage serviceSave) 
        : IServiceTelemetry
    {
        #region private properties
        private double lastPointAltitude = 0;
        #endregion


        #region private readonly properties
        private readonly IServiceSaveParametrage _serviceSave = serviceSave;
        #endregion

        #region public method interface IServiceTelemetry
        public void CalculateTelemetry(Localisation first, Localisation second
            , ref RouteTelemetry telemetry)
        {
            // -- calcul la distance selon DistanceUnit 
            var p1 = first.GetLocation();
            var p2 = second.GetLocation();
            telemetry.Distance += Location.CalculateDistance(p1, p2, telemetry.DistanceUnit);
            
            // -- calcul le temps passé
            telemetry.TotalTime += (p2.Timestamp - p1.Timestamp).TotalSeconds;

            // -- calcul vitesse moyenne km/h ou miles/h
            telemetry.AverageSpeed = (telemetry.Distance / telemetry.TotalTime) * 3600;
            
            // -- calcul élévation 
            double elevation = CalculElevation(first, second);
            if ( elevation > 0 ) telemetry.PositiveElevationGain += elevation;
            else telemetry.NegativeElevationGain += Math.Abs(elevation);

        }


        public RouteTelemetry CalculateTelemetry(List<Localisation> trackPoints)
        {
            var result = new RouteTelemetry();
            var IsMetric = (bool)_serviceSave.GetParam("IsMetric", true);

            if (!IsMetric) result.DistanceUnit = DistanceUnits.Miles;

            if ( trackPoints.Count > 1 )
            {
                for(int i = 1; i < trackPoints.Count; i++)
                {
                    CalculateTelemetry ( trackPoints[i-1], trackPoints[i], ref result);
                }
            }

            result.TotalLocalisations = trackPoints.Count;

            return result;
        }
        #endregion

        #region private method 
        private double CalculElevation(Localisation first, Localisation second) { 
        
            if (first.Altitude is not null )
            {
                if (second.Altitude is not null)
                {
                    lastPointAltitude = (double)second.Altitude;
                    return (double)(second.Altitude - first.Altitude);
                }
                else return 0;
            }else if ( lastPointAltitude > 0 && second.Altitude is not null )
            {
                return (double)second.Altitude - lastPointAltitude;
            }

            return 0;
        }
        #endregion
    }
}
