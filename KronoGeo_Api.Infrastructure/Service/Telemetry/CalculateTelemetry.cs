using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Service.Telemetry
{
    public class CalculateTelemetry : IServiceTelemetry
    {
        void IServiceTelemetry.CalculateTelemetry(Localisation first, Localisation second, out RouteTelemetry telemetry)
        {
            throw new NotImplementedException();
        }

        RouteTelemetry IServiceTelemetry.CalculateTelemetry(ICollection<Localisation> localisations)
        {
            throw new NotImplementedException();
        }
    }
}
