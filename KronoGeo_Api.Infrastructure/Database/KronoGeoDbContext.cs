using KronoGeo_Api.Infrastructure.Database.TypeConfiguration;
using KronoGeo_Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database
{
    public class KronoGeoDbContext : IdentityDbContext<ApplicationUser>
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
        public DbSet<Localisation> Localisations { get; set; }
        public DbSet<LocalisationPhoto> LocalisationPhotos { get; set; }
        public DbSet<LocalisationGroup> LocalisationGroups { get; set; }
        public DbSet<RouteTelemetry> RouteTelemetries { get; set; }
        #endregion

        #region protected overrides
        /// <summary>
        /// methode qui va charger les models de création des différentes tables dans la base de données
        /// </summary>
        /// <param name="builder"></param>
        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // -- Apply configurations 
            builder.ApplyConfiguration(new LocalisationEntityTypeConfiguration());
            builder.ApplyConfiguration(new LocalisationGroupEntityTypeConfiguration());
            builder.ApplyConfiguration(new RouteTelemetryEntityTypeConfiguration());
        }
        #endregion
    }
}
