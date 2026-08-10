using KronoGeo_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database.TypeConfiguration
{
    internal class LocalisationGroupEntityTypeConfiguration
        : IEntityTypeConfiguration<LocalisationGroup>
    {
        public void Configure(EntityTypeBuilder<LocalisationGroup> builder)
        {
            builder.ToTable(nameof(LocalisationGroup));
            builder.HasKey(lg => lg.Id);
            builder.Property(lg => lg.Id).ValueGeneratedOnAdd();

            builder.Property(lg => lg.Name).IsRequired();
            builder.Property(lg => lg.Date).IsRequired();

            // - clé étrangère 
            builder.HasOne(lg => lg.ApplicationUser).WithMany(lg => lg.LocalisationGroups);
            builder.HasOne(lg => lg.RouteTelemetry).WithOne(rt => rt.LocalisationGroup)
                .HasForeignKey<LocalisationGroup>(lg => lg.RouteTelemetryId);
        }
    }
}
