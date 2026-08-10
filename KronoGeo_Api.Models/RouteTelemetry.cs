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
        public double AverageHeartRate { get; set; } = 0;
        public DateTimeOffset DateTimeBegin { get; set; } = DateTimeOffset.Now;
        public DateTimeOffset DateTimeEnd { get; set; } = DateTimeOffset.Now;
        public int TotalTimePaused { get; set; } = 0;
        public int TotalLocalisations { get; set; } = 0;

        public int LocalisationGroupId { get; set; }
        public LocalisationGroup? LocalisationGroup { get; set; }
    }
}
