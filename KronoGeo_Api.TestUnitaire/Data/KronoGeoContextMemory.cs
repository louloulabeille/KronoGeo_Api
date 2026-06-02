using KronoGeo_Api.Infrastructure.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.TestUnitaire.Data
{
    // création d'une classe de contexte de base de données en mémoire pour les tests unitaires
    // cette classe hérite de la classe de contexte de base de données réelle
    // et utilise la méthode OnConfiguring pour configurer une base de données en mémoire
    public class KronoGeoContextMemory : KronoGeoDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseInMemoryDatabase(databaseName: "KronoGeoDbTest");
        }
    }
}
