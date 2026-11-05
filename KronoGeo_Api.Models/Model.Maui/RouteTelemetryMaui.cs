using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Maui.Devices.Sensors;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.Maui
{
    [Table("RouteTelemetry")]
    public class RouteTelemetryMaui
    {
        [SQLite.PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public double Distance { get; set; } = 0;
        public DistanceUnits DistanceUnit { get; set; } = DistanceUnits.Kilometers;
        public double AverageSpeed { get; set; } = 0;
        public double PositiveElevationGain { get; set; } = 0;
        public double NegativeElevationGain { get; set; } = 0;
        public DateTimeOffset DateTimeBegin { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset DateTimeEnd { get; set; } = DateTimeOffset.Now;
        public double TotalTime { get; set; } = 0;
        public double TotalTimePaused { get; set; } = 0;
        public int TotalLocalisations { get; set; } = 0;

        #region public method conversion
        /// <summary>
        /// Retourne la RouteTelemetry
        /// </summary>
        /// <returns></returns>
        public RouteTelemetryDTO ToLocalisation()
        {
            return new()
            {
                Distance = this.Distance,
                DistanceUnit = this.DistanceUnit,
                AverageSpeed = this.AverageSpeed,
                PositiveElevationGain = this.PositiveElevationGain,
                NegativeElevationGain = this.NegativeElevationGain,
                DateTimeBegin = this.DateTimeBegin,
                DateTimeEnd = this.DateTimeEnd,
                TotalTime = this.TotalTime,
                TotalTimePaused = this.TotalTimePaused,
                TotalLocalisations = this.TotalLocalisations
            };
        }

        /// <summary>
        /// retourne la RouteTeletryMaui à partir d'un RouteTeletry
        /// </summary>
        /// <param name="routeTelemetry"></param>
        /// <returns></returns>
        public static RouteTelemetryMaui FromRouteTelemetry(RouteTelemetryDTO routeTelemetry)
        {
            return new()
            {
                Distance        = routeTelemetry.Distance,
                DistanceUnit    = routeTelemetry.DistanceUnit,
                AverageSpeed    = routeTelemetry.AverageSpeed,
                PositiveElevationGain = routeTelemetry.PositiveElevationGain,
                NegativeElevationGain = routeTelemetry.NegativeElevationGain,
                DateTimeBegin   = routeTelemetry.DateTimeBegin,
                DateTimeEnd     = routeTelemetry.DateTimeEnd,
                TotalTime       = routeTelemetry.TotalTime,
                TotalTimePaused = routeTelemetry.TotalTimePaused,
                TotalLocalisations = routeTelemetry.TotalLocalisations
            };
        }
        #endregion
    }
}
