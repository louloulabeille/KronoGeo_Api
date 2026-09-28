using KronoGeo_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database.TypeConfiguration
{
    internal class LocalisationPhotoEntityTypeConfiguration : IEntityTypeConfiguration<LocalisationPhoto>
    {
        public void Configure(EntityTypeBuilder<LocalisationPhoto> builder)
        {
            // -- configuration spécifique pour LocalisationPhoto type text 1Go max
            builder.Property(lp => lp.Description).HasColumnType("text");
        }
    }
}
