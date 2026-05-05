using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database
{
    public class KronoGeoDbContext : IdentityDbContext
    {
        #region constructeur
        public KronoGeoDbContext(DbContextOptions<KronoGeoDbContext> options) : base(options)
        {
        }

        protected KronoGeoDbContext()
        {
        }
        #endregion


        #region dbSet

        #endregion

        #region protected overrides
        /// <summary>
        /// methode qui va charger les models de création des différentes tables dans la base de données
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
        #endregion
    }
}
