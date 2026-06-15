using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Model.DTO;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class InMemoriMauiUser : IServiceSaveUser
    {
        public bool ClearUser()
        {
            throw new NotImplementedException();
        }

        public RegisterDTO GetRegister()
        {
            throw new NotImplementedException();
        }

        public async Task SaveUser(RegisterDTO register)
        {
            DeleteUser();   // - supprime

            await SecureStorage.Default.SetAsync("Id-User", register.Id);
            await SecureStorage.Default.SetAsync("Login-User", register.Login);
            await SecureStorage.Default.SetAsync("Password-User", register.Id);
            await SecureStorage.Default.SetAsync("Email-User", register.Id);
            await SecureStorage.Default.SetAsync("PhoneNumber-User", register.Id);
            await SecureStorage.Default.SetAsync("Token-User", register.Id);
        }

        /// <summary>
        /// supprime dans le storage securisé les informations de l'user
        /// </summary>
        private void DeleteUser()
        {
            SecureStorage.Default.Remove("Id-User");
            SecureStorage.Default.Remove("Login-User");
            SecureStorage.Default.Remove("Password-User");
            SecureStorage.Default.Remove("Email-User");    
            SecureStorage.Default.Remove("PhoneNumber-User");
            SecureStorage.Default.Remove("Token-User");

        }
    }
}
