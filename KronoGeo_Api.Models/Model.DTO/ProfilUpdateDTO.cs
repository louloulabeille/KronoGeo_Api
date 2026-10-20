using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace KronoGeo_Api.Models.Model.DTO
{
    public class ProfilUpdateDTO
    {
        #region private properties
        private string? _phoneNumber;
        #endregion


        [Required(ErrorMessage ="Le login ou le nom d'utilisateur est obligatoire.")]
        public string? Login { get; set; }
        [Required(ErrorMessage = "L'adresse mail est obligatoire.")]
        [EmailAddress(ErrorMessage = "L'adresse mail est invalide.")]
        public string? Email { get; set; }
        [Phone(ErrorMessage ="Le numéro de téléphone est invalide.")]
        public string? PhoneNumber {
            get {
                return _phoneNumber;    
            }
            set {
                _phoneNumber = string.IsNullOrEmpty(value) ? null : value;
            } 
        }
    }
}
