using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database
{
    internal class KronoGeoContextFactory : IDesignTimeDbContextFactory<KronoGeoDbContext>
    {
        public KronoGeoDbContext CreateDbContext(string[] args)
        {
            var optionBuilder = new DbContextOptionsBuilder<KronoGeoDbContext>();
            optionBuilder.UseNpgsql(@"Server=127.0.0.1;Port=5432;Database=KronoGeo;User Id=krono;Password=ieupn486jadF&;");
            return new KronoGeoDbContext(optionBuilder.Options);
        }
    }
}
