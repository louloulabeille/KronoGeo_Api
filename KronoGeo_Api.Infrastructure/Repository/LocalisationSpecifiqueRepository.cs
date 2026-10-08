using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Repository
{
    /// <summary>
    /// Repository spécifique pour la gestion des entités Localisation.
    /// </summary>
    /// <param name="context"></param>
    public class LocalisationSpecifiqueRepository (KronoGeoDbContext context)
        : Repository<Localisation>(context), ILocalisationSpecifiqueRepository
    {
        public bool ConvertLocalisationPhotoToLocalisation(int idPhoto)
        {

            // Détacher l'entité si elle est déjà suivie par le contexte
            var tracked = Context.ChangeTracker.Entries<Localisation>()
                .FirstOrDefault(e => e.Entity.Id == idPhoto);

            if (tracked is not null)
                tracked.State = EntityState.Detached;

            var rows = Context.Database.ExecuteSqlInterpolated($"""UPDATE "Localisation" SET "TypeLocalisation" = 'Default' WHERE "Id" = {idPhoto} AND "TypeLocalisation" = 'Photo'""");

            return rows > 0;
        }

 
    }
}
