using KronoGeo_Api.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.ExtendMethods
{
    /// <summary>
    /// methode d'extension pour la classe Localisation de l'API KronoGeo
    /// impossible de l'ajouter à la classe car Location est une classe de l'API KronoGeo Maui
    /// et n'existe pas en dehors de l'application Maui
    /// </summary>
    public static class LocationExtend
    {
        extension (Localisation location)
        {
            public Location GetLocation()
            {
                return new(location.Latitude, location.Longitude)
                {
                    Altitude = location.Altitude,
                    Accuracy = location.Accuracy,
                    Course = location.Course,
                    Speed = location.Speed,
                    Timestamp = location.Timestamp,
                    VerticalAccuracy = location.VerticalAccuracy
                };
            }
            
        }
    }
}
