using KronoGeo_Api.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.Maui.Devices.Sensors;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class RouteTelemetryViewBase : ComponentBase
    {
        #region private parameters properties
        [Parameter]
        public RouteTelemetry? Telemetry { get; set; } = default;
        #endregion

        #region protected properties view
        protected string Distance { get; set; } = string.Empty;
        protected string AverageSpeed { get; set; } = string.Empty;
        protected string PositiveElevationGain { get; set; } = string.Empty;
        protected string NegativeElevationGain { get; set; } = string.Empty;
        protected string DateTimeBegin { get; set; } = string.Empty;
        protected string DateTimeEnd { get; set; } = string.Empty;
        protected string TotalTime { get; set; } = string.Empty;
        protected string TotalTimePaused { get; set; } = string.Empty;
        
        #endregion

        #region protected override methods
        protected override void OnInitialized()
        {
            LoadTelemetryView();

            base.OnInitialized();
        }
        #endregion

        #region private methods
        private void LoadTelemetryView()
        {
            if (Telemetry is not null)
            {
                string unitDistance = Telemetry.DistanceUnit == DistanceUnits.Kilometers ? "km" : "m";
                string unitSpeed = Telemetry.DistanceUnit == DistanceUnits.Kilometers ? "km/h" : "m/s";
                string unitElevation = Telemetry.DistanceUnit == DistanceUnits.Kilometers ? "m" : "ft";

                Distance = $"{Math.Round(Telemetry.Distance, 3)} {unitDistance}";
                AverageSpeed = $"{Math.Round(Telemetry.AverageSpeed, 3)} {unitSpeed}";
                PositiveElevationGain = $"{Math.Round(Telemetry.PositiveElevationGain, 2)} {unitElevation}";
                NegativeElevationGain = $"{Math.Round(Telemetry.NegativeElevationGain, 2)} {unitElevation}";
                DateTimeBegin = Telemetry.DateTimeBegin.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                DateTimeEnd = Telemetry.DateTimeEnd.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                TotalTime = Telemetry.TotalTime.ToString();
                TotalTimePaused = Telemetry.TotalTimePaused.ToString();
            }
        }
        #endregion
    }
}
