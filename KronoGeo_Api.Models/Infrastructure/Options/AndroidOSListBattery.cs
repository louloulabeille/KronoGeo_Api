using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Models.Infrastructure.Options
{
    /// <summary>
    /// liste des Os devices Android qui ont une gestion agressive sur la batterie 
    /// qui empêche l'utilisation d'un foregroung service d'android pour la géolocation en arrière plan
    /// </summary>
    public class AndroidOSListBattery
    {
        public string[] ListOs { get; set; } = [];
    }
}
