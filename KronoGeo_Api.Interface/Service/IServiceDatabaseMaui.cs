using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Model.Maui;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceDatabaseMaui
    {
        public Task<List<LocalisationMaui>> GetAllAsync();
        public Task<int> Count();
        public Task Clear();
        public Task<int> SaveAsync(LocalisationMaui p);
        public Task<int> DeleteAsync(LocalisationMaui p);
        public Task<bool> Exists(LocalisationMaui p);

        // -- j'aurai pu le mettre dans un autre service
        public Task<RouteTelemetryMaui> GetTelemetryAsync();
        public Task<int> SaveTelemetryAsync(RouteTelemetryMaui p);
        public Task<int> DeleteTelemetryAsync(RouteTelemetryMaui p);

    }
}
