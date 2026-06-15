using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Http
{
    public enum EnumApiStatus
    {
        Success,
        BadRequest,
        Problem
    }

    public abstract class ResponseApi
    {
        public EnumApiStatus ApiStatus { get; set; } = EnumApiStatus.Success;
        public string? Message { get; set; }

        /// <summary>
        /// retourne le status succes quand tout c'est bien passé
        /// </summary>
        /// <returns></returns>
        public bool IsSuccess { get { return ApiStatus == EnumApiStatus.Success; }}
        /// <summary>
        /// retour le status problem qui correspond à une levée d'exception au niveau de Api
        /// </summary>
        /// <returns></returns>
        public bool IsProblem { get { return ApiStatus == EnumApiStatus.Problem; } }
        /// <summary>
        /// retourne bad request quand la requête ne peut pas être évaluée ou le resultat est mauvaise 
        /// </summary>
        /// <returns></returns>
        public bool IsBadRequest { get { return ApiStatus == EnumApiStatus.BadRequest; } }
    }
}
