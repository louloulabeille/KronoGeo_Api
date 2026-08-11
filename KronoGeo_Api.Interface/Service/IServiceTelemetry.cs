using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceTelemetry
    {
        //public void SaveTelemetry(RouteTelemetry telemetry);
        /// <summary>
        /// calcule la télémétrie entre deux localisations et le retourne 
        /// à partir d'une télémetrie existante, la met à jour avec les nouvelles valeurs calculées
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="telemetry"></param>
        public void CalculateTelemetry(Localisation first, Localisation second, out RouteTelemetry telemetry);
        /// <summary>
        /// calcule la télémétrie à partir d'une collection de localisations et retourne un objet RouteTelemetry
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        public RouteTelemetry CalculateTelemetry(ICollection<Localisation> localisations);
    }
}
