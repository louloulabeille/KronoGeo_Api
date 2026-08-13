using KronoGeo_Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database.TypeConfiguration
{
    public class RouteTelemetryEntityTypeConfiguration :
        IEntityTypeConfiguration<RouteTelemetry>
    {
        public void Configure(EntityTypeBuilder<RouteTelemetry> builder)
        {
            builder.ToTable(nameof(RouteTelemetry));
            builder.HasKey(rt => rt.Id);
            builder.Property(rt => rt.Id).ValueGeneratedOnAdd();

            builder.HasOne(rt => rt.LocalisationGroup).WithOne(lg => lg.RouteTelemetry)
                .HasForeignKey<RouteTelemetry>(rt => rt.LocalisationGroupId);
        }
    }
}
