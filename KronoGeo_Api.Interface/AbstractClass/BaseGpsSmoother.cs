using Microsoft.Maui.Devices.Sensors;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.AbstractClass
{
    public abstract class BaseGpsSmoother
    {
        #region const properties
        /// <summary>
        /// constante de Excellent accuracy,  toutes les valeurs en dessous sont ne sont pas modifiées
        /// en mètre
        /// </summary>
        public const double ExcellentAccuracy = 8.0;
        /// <summary>
        /// constante max acceptable accurancy en mètre
        /// </summary>
        internal const double MaxConst = 30.00;
        #endregion

        #region public properties
        /// <summary>
        /// properties de max acceptable Accuracy  - avec pour valeur MaxConst 30 mètres
        /// </summary>
        public double MaxAcceptableAccuracy { get; set; } = MaxConst;
        #endregion

        #region method public
        public abstract Location? AcceptableLocationCalcul(Location newLocation);
        public abstract void Reset();
        #endregion
    }
}
