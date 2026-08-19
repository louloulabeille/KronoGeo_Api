using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceTelemetry
    {
        #region public properties const
        public const double BruitGps = 3.00; // -- >= 3.0 mètre calcul de l'élévation
        #endregion

        //public void SaveTelemetry(RouteTelemetry telemetry);
        /// <summary>
        /// calcule la télémétrie entre deux localisations et le retourne 
        /// à partir d'une télémetrie existante, la met à jour avec les nouvelles valeurs calculées
        /// </summary>
        /// <param name="first"></param>
        /// <param name="second"></param>
        /// <param name="telemetry"></param>
        public void CalculateTelemetry(Localisation first, Localisation second, ref RouteTelemetry telemetry);
        /// <summary>
        /// calcule la télémétrie à partir d'une collection de localisations et retourne un objet RouteTelemetry
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        public RouteTelemetry CalculateTelemetry(List<Localisation> localisations);
    }
}
