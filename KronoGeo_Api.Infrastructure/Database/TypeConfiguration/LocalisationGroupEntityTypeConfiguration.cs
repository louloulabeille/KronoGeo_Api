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
            /*var dateTimeConverter = new ValueConverter<DateTime, DateTime>(
            v => v.Kind == DateTimeKind.Utc ? v : v.ToUniversalTime(),
            v => DateTime.SpecifyKind(v, DateTimeKind.Utc));*/

            builder.ToTable(nameof(LocalisationGroup));
            builder.HasKey(lg => lg.Id);
            builder.Property(lg => lg.Id).ValueGeneratedOnAdd();

            builder.Property(lg => lg.Name).IsRequired();
            builder.Property(lg => lg.Date).IsRequired();
                //.HasConversion(dateTimeConverter); ;

            // - clé étrangère 
            builder.HasOne(lg => lg.ApplicationUser).WithMany(lg => lg.LocalisationGroups);
        }
    }
}
