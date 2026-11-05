using KronoGeo_Api.Models.Model.DTO;
using Microsoft.EntityFrameworkCore;
using Microsoft.Maui.Devices.Sensors;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Model.Maui
{
    [Table("Localisations")]
    public class LocalisationMaui
    {
        [SQLite.PrimaryKey, AutoIncrement]
        public int Id { get; set; }
        public int OrderIndex { get; set; } = 0;   // -- compteur des localisations pour le tri
        public DateTimeOffset Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        /// <summary>
        /// en mètre sous android avec comme point de référence WGS 84 plus ou moins 45 à 50 mètres plus haut
        /// </summary>
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public double? VerticalAccuracy { get; set; }
        public double? Speed { get; set; }
        public double? Course { get; set; }

        // -- pour les photos héritage de LocalisationMauiPhoto
        public string? Name { get; set; } = string.Empty;
        public string? PathPhoto { get; set; }
        public string? Description { get; set; }


        #region public method conversion
        /// <summary>
        /// Retourne une localisation à partir de cette object
        /// </summary>
        /// <returns></returns>
        public virtual LocalisationDTO ToLocalisation()
        {
            if ( Name is not null )
            {
                return new LocalisationPhotoDTO()
                {
                    OrderIndex = this.OrderIndex,
                    Timestamp = this.Timestamp,
                    Latitude = this.Latitude,
                    Longitude = this.Longitude,
                    Altitude = this.Altitude,
                    Accuracy = this.Accuracy,
                    VerticalAccuracy = this.VerticalAccuracy,
                    Speed = this.Speed,
                    Course = this.Course,
                    Name = this.Name,
                    PathPhoto = this.PathPhoto,
                    Description = this.Description
                };
            }
            return new ()
            {
                //Id = this.Id,
                OrderIndex = this.OrderIndex,
                Timestamp = this.Timestamp,
                Latitude = this.Latitude,
                Longitude = this.Longitude,
                Altitude = this.Altitude,
                Accuracy = this.Accuracy,
                VerticalAccuracy = this.VerticalAccuracy,
                Speed = this.Speed,
                Course = this.Course
            };
        }

        public Location GetLocation()
        {
            return new Location(Latitude, Longitude)
            {
                Accuracy = Accuracy,
                Altitude = Altitude,
                VerticalAccuracy = VerticalAccuracy,
                Course = Course,
                Speed = Speed,
                Timestamp = Timestamp,
#if ANDROID
                    AltitudeReferenceSystem = AltitudeReferenceSystem.Ellipsoid,
#endif
#if IOS
                    AltitudeReferenceSystem = AltitudeReferenceSystem.Geoid,
#endif
            };
        }

        /// <summary>
        /// Convertie une localisation en LocalisationMaui
        /// </summary>
        /// <param name="localisation"></param>
        /// <returns></returns>
        public static LocalisationMaui FromLocalisation(LocalisationDTO localisation)
        {
            var result = new LocalisationMaui()
            {
                Id = localisation.Id,
                OrderIndex = localisation.OrderIndex,
                Timestamp = localisation.Timestamp,
                Latitude = localisation.Latitude,
                Longitude = localisation.Longitude,
                Altitude = localisation.Altitude,
                Accuracy = localisation.Accuracy,
                VerticalAccuracy = localisation.VerticalAccuracy,
                Speed = localisation.Speed,
                Course = localisation.Course
            };

            if (localisation is LocalisationPhotoDTO photo )
            {
                result.Name = photo.Name;
                result.PathPhoto = photo.PathPhoto;
                result.Description = photo.Description;
            }

            return result;
        }
        #endregion

    }
}
