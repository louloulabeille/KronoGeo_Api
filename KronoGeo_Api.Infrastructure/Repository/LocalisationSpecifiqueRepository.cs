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
        /// <summary>
        /// supprime la localisation Photo pour la transformer en localisation normale
        /// </summary>
        /// <param name="idPhoto"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Repository spécifique pour retourner les géolocalisations 
        /// avec une jointure
        /// </summary>
        /// <param name="idUSer"></param>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <param name="idIgnore"></param>
        /// <returns></returns>
        public async Task<IEnumerable<LocalisationPhoto>>
            GetLocalisationPhotoByUserIdAndMinMaxLatitude(string idUSer, double minLat, double maxLat, int idIgnore)
        {
            if ( idIgnore == 0 )
            {
                return await Context.LocalisationPhotos
                .Include(p => p.LocalisationGroup)
                .Where(p => p.LocalisationGroup != null &&
                p.LocalisationGroup.ApplicationUserId == idUSer
                && p.Latitude >= minLat && p.Latitude <= maxLat).ToListAsync();
            }
            else
            {   // -- pour ignorer LocalisationGroupId qui est déjà affiché dans la Map
                return await Context.LocalisationPhotos
                .Include(p => p.LocalisationGroup)
                .Where(p => p.LocalisationGroup != null &&
                p.LocalisationGroup.ApplicationUserId == idUSer
                && p.Latitude >= minLat && p.Latitude <= maxLat 
                && p.LocalisationGroupId != idIgnore).ToListAsync();
            }

            
        }
    }
}
