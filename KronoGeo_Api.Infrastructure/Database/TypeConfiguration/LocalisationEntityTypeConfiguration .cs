using KronoGeo_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database.TypeConfiguration
{
    internal class LocalisationEntityTypeConfiguration : IEntityTypeConfiguration<Localisation>
    {
        public void Configure(EntityTypeBuilder<Localisation> builder)
        {
            builder.ToTable(nameof(Localisation));
            builder.HasKey(l => l.Id);
            builder.Property(l => l.Id).ValueGeneratedOnAdd();

            builder.Property(l => l.Timestamp).IsRequired();
            builder.Property(l => l.Latitude).IsRequired();
            builder.Property(l => l.Longitude).IsRequired();

            // -- clé étrangère
            builder.HasOne(l => l.LocalisationGroup).WithMany(l => l.Localisations);

            // -- mise en place du STI Single Table Inheritance ou TPH table per hierarchy
            // -- heritage par table unique - normalement il y aura la vidéo maybe
            builder.HasDiscriminator<string>("TypeLocalisation")
                .HasValue<Localisation>("Default")
                .HasValue<LocalisationPhoto>("Photo");

            
        }
    }
}
