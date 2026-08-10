using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    /// <summary>
    /// classe d'extension de la classe Localisation
    /// ajout de méthod d'extension pour la classe Localisation
    /// </summary>
    internal static class LocalisationExtend
    {
        extension (Localisation localisation)
        {
            /// <summary>
            /// A partir d'une localisation, retourne un objet Location de MAUI
            /// pour faire des calculs de distance, de vitesse, etc...
            /// </summary>
            /// <returns></returns>
            public Location GetLocation()
            {
                return new Location(localisation.Latitude, localisation.Longitude)
                {
                    Accuracy = localisation.Accuracy,
                    Altitude = localisation.Altitude,
                    VerticalAccuracy = localisation.VerticalAccuracy,
                    Course = localisation.Course,
                    Speed = localisation.Speed,
                    Timestamp = localisation.Timestamp,
#if ANDROID
                    AltitudeReferenceSystem = AltitudeReferenceSystem.Ellipsoid,
#endif
#if IOS
                    AltitudeReferenceSystem = AltitudeReferenceSystem.Geoid,
#endif
                };
            }
        }
    }
}
