using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace KronoGeo_Api.Models.Model.DTO
{
    // -- Json serialization properties TypeObjet
    // -- pas de TypeObject LocalisationDTO sinon TypeObject = 1 => LocalisationPhotoDTO
    [JsonPolymorphic(TypeDiscriminatorPropertyName = "TypeObject")]
    [JsonDerivedType(typeof(LocalisationPhotoDTO), typeDiscriminator:1)]
    public class LocalisationDTO
    {
        #region public protperties
        //public TypeLocalisation TypeObjet { get; set; } = TypeLocalisation.Base;
        public int Id { get; set; }
        public int OrderIndex { get; set; } = 0; // -- compteur des localisations
        public DateTimeOffset Timestamp { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double? Altitude { get; set; }
        public double? Accuracy { get; set; }
        public double? VerticalAccuracy { get; set; }
        public double? Speed { get; set; }
        public double? Course { get; set; }
        #endregion

        #region public method
        /// <summary>
        /// retourne un object LocalisationPhoto ou LocalisationDTO
        /// avec un object Localisation en entré
        /// </summary>
        /// <param name="localisation"></param>
        /// <returns></returns>
        public static LocalisationDTO Parse (Localisation localisation)
        {
            if (localisation is LocalisationPhoto photo)
            {
                return new LocalisationPhotoDTO() {
                    Id          = photo.Id,
                    OrderIndex  = photo.OrderIndex,
                    Latitude    = photo.Latitude,
                    Longitude   = photo.Longitude,
                    Accuracy    = photo.Accuracy,
                    Altitude    = photo.Altitude,
                    Course      = photo.Course,
                    Speed       = photo.Speed,
                    VerticalAccuracy = photo.VerticalAccuracy,
                    Timestamp   = photo.Timestamp.ToUniversalTime(),
                    Name        = photo.Name,
                    PathPhoto   = photo.PathPhoto
                };
            }
            else
            return new LocalisationDTO()
            {
                Id          = localisation.Id,
                OrderIndex  = localisation.OrderIndex,
                Latitude    = localisation.Latitude,
                Longitude   = localisation.Longitude,
                Accuracy    = localisation.Accuracy,
                Altitude    = localisation.Altitude,
                Course      = localisation.Course,
                Speed       = localisation.Speed,
                VerticalAccuracy = localisation.VerticalAccuracy,
                Timestamp   = localisation.Timestamp.ToUniversalTime()
            };
        }
        #endregion
    }
}
