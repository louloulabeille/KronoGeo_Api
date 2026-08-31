using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Parameter
{
    /// <summary>
    /// class qui retour les informations compris dans le session de l'utilisateur
    /// qui est sera lu dans le cookie HttpOnly sécurisé pour le coté web assembly
    /// </summary>
    public class UserInfos
    {
        public bool IsAuthenticate { get; set; } = false;
        public required string Id { get; set; }
        public string Login { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = [];
        public Dictionary<string, string> Claims { get; set; } = [];

    }
}
