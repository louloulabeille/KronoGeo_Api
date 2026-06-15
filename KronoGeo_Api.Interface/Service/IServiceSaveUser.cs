using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    /// <summary>
    /// Interface pour mettre en place la gestion du user dans l'application
    /// sauvegarder - supprimer - et modifier
    /// </summary>
    public interface IServiceSaveUser
    {
        /// <summary>
        /// Enregistre le compte utilisateur dans l'application
        /// </summary>
        /// <param name="register"></param>
        /// <returns></returns>
        public Task SaveUser(RegisterDTO register);
        /// <summary>
        /// Supprime l'utilisateur dans l'apllication
        /// </summary>
        /// <returns></returns>
        public void ClearUser();
        /// <summary>
        /// retourne l'utilisateur dans l'apllication
        /// </summary>
        /// <returns></returns>
        public Task<RegisterDTO?> GetRegister();
    }
}
