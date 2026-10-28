using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.ValidationsAttributes
{
    public class NotEqualToAttribute : ValidationAttribute
    {
        private readonly string _otherProperty;

        public NotEqualToAttribute(string otherProperty)
        {
            _otherProperty = otherProperty;
        }

        /// <summary>
        /// Valid si 2 properties sont égaux qui vrai alors erreur
        /// </summary>
        /// <param name="value"></param>
        /// <param name="validationContext"></param>
        /// <returns></returns>
        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            var otherPropertyInfo = validationContext.ObjectType.GetProperty(_otherProperty);
            if (otherPropertyInfo == null)
                return new ValidationResult($"Propriété inconnue : {_otherProperty}");

            var otherValue = otherPropertyInfo.GetValue(validationContext.ObjectInstance, null);

            if (Equals(value, otherValue))
                return new ValidationResult(ErrorMessage ?? "Les valeurs doivent être différentes.");

            return ValidationResult.Success;
        }
    }
}
