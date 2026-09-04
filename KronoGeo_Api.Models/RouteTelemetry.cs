using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models
{
    public class RouteTelemetry
    {
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

        public int LocalisationGroupId { get; set; }
        public LocalisationGroup? LocalisationGroup { get; set; }

        #region public method
        public RouteTelemetryDTO GetDTO()
        {
            return new RouteTelemetryDTO()
            {
                Id = this.Id,
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
        #endregion

    }
}
