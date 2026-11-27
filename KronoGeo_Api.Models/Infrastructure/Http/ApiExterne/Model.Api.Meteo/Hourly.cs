using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http.ApiExterne.Model.Api.Meteo
{
    /// <summary>
    /// class à mettre en jour selon le ParamMeteo.Hourly
    /// </summary>
    public class Hourly
    {
        public List<DateTime?>? Time { get; set; }
        /// <summary>
        /// Température à 2 mètres du sol
        /// </summary>
        public List<float?>? Temperature_2m { get; set; }
        /// <summary>
        /// Taux d'humidité en % 0 - 100%
        /// </summary>
        public List<int?>? Relative_humidity_2m { get; set; }
        /// <summary>
        /// Température du point de rosée à 2 mètres du sol
        /// </summary>
        public List<float?>? Dew_point_2m { get; set; }
        /// <summary>
        /// Température Perçu par l'homme
        /// </summary>
        public List<float?>? Apparent_temperature { get; set; }
        /// <summary>
        /// Vitesse du vent à 10 mètres du sol
        /// </summary>
        public List<float?>? Wind_speed_10m { get; set; }
        /// <summary>
        /// Quantité de précipitation
        /// </summary>
        public List<float?>? Precipitation { get; set; }
        /// <summary>
        /// Pression atmosphérique en surface
        /// </summary>
        public List<float?>? Surface_pressure { get; set; }
        /// <summary>
        /// code de la météo 
        /// </summary>
        public List<WeatherCode?>? Weather_code { get; set; }
    }


    public enum WeatherCode
    {
        [Description("Ciel dégagé")]
        CielDegage = 0,
        [Description("Ciel principalement dégagé")]
        CielPrincipalementDegage = 1,
        [Description("Ciel partiellement nuageux")]
        CielPartiellementNuageux = 2,
        [Description("Ciel couvert")]
        CielCouvert = 3,
        [Description("Brouillard")]
        Brouillard = 45,
        [Description("Brouillard givrant")]
        BrouillardGivrant = 48,
        [Description("Bruine légère")]
        BruineLegere = 51,
        [Description("Bruine modérée")]
        BruineModeree = 53,
        [Description("Bruine dense")]
        BruineDense = 55,
        [Description("Bruine verglaçante légère")]
        BruineVerglacanteLegere = 56,
        [Description("Bruine verglaçante dense")]
        BruineVerglacanteDense = 57,
        [Description("Pluie légère")]
        PluieLegere = 61,
        [Description("Pluie modérée")]
        PluieModeree = 63,
        [Description("Pluie forte")]
        PluieForte = 65,
        [Description("Légère pluie verglaçante")]
        PluieVerglacanteLegere = 66,
        [Description("Forte pluie verglaçante")]
        PluieVerglacanteForte = 67,
        [Description("Légères chutes de neige")]
        NeigeLegere = 71,
        [Description("Chutes de neige modérées")]
        NeigeModeree = 73,
        [Description("Chutes de neige abondantes")]
        NeigeAbondante = 75,
        [Description("Grains de neige")]
        GrainsDeNeige = 77,
        [Description("Légères averses de pluie")]
        AverseLegere = 80,
        [Description("Averses de pluie modérées")]
        AverseModeree = 81,
        [Description("Averses de pluie violentes")]
        AverseViolente = 82,
        [Description("Légères averses de neige")]
        AverseNeigeLegere = 85,
        [Description("Fortes averses de neige")]
        AverseNeigeForte = 86,
        [Description("Orage")]
        Orage = 95,
        [Description("Orage accompagné de grêlons légers")]
        OrageGrelonsLegers = 96,
        [Description("Orage violent")]
        OrageViolent = 97,
        [Description("Orage accompagné de gros grêlons")]
        OrageGrosGrelons = 99
    }
}
