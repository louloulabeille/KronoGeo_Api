using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class InMemoriMauiUser : IServiceSaveUser
    {
        public void ClearUser()
        {
            DeleteUser();
        }

        /// <summary>
        /// retour l'utilisateur à partir des infos enregistrés en mémoire de l'application 
        /// </summary>
        /// <returns></returns>
        public async Task<RegisterDTO?> GetRegister()
        {
            var id = await SecureStorage.Default.GetAsync("Id-User");
            var login = await SecureStorage.Default.GetAsync("Login-User");
            var pass = await SecureStorage.Default.GetAsync("Password-User");
            var email = await SecureStorage.Default.GetAsync("Email-User");
            var phone = await SecureStorage.Default.GetAsync("Phone-User");
            var token = await SecureStorage.Default.GetAsync("Token-User");

            if (id != null && login != null &&  pass != null && email != null)
            {
                var user = new RegisterDTO
                {
                    Id = id,
                    Login = login,
                    Password = pass,
                    Email = email,
                    PhoneNumber = phone ?? string.Empty,
                    Token = token ?? string.Empty
                };

                return user;
            }
            return null;
        }

        public async Task SaveUser(RegisterDTO register)
        {
            DeleteUser();   // - supprime le user par défaut

            await SecureStorage.Default.SetAsync("Id-User", register.Id);
            await SecureStorage.Default.SetAsync("Login-User", register.Login);
            await SecureStorage.Default.SetAsync("Password-User", register.Id);
            await SecureStorage.Default.SetAsync("Email-User", register.Id);
            await SecureStorage.Default.SetAsync("Phone-User", register.Id);
            await SecureStorage.Default.SetAsync("Token-User", register.Id);
        }

        /// <summary>
        /// supprime dans le storage securisé les informations de l'user
        /// </summary>
        private static void DeleteUser()
        {
            SecureStorage.Default.Remove("Id-User");
            SecureStorage.Default.Remove("Login-User");
            SecureStorage.Default.Remove("Password-User");
            SecureStorage.Default.Remove("Email-User");    
            SecureStorage.Default.Remove("Phone-User");
            SecureStorage.Default.Remove("Token-User");

        }
    }
}
