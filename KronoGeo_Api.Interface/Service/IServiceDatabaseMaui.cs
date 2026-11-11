using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Model.Maui;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Interface.Service
{
    public interface IServiceDatabaseMaui
    {
        // -- Table LocalisationMaui
        public Task<List<LocalisationMaui>> GetAllAsync();
        public Task<int> Count();
        public Task Clear();
        public Task<int> SaveAsync(LocalisationMaui p);
        public Task<int> DeleteAsync(LocalisationMaui p);
        public Task<bool> Exists(LocalisationMaui p);

        // -- Table RouteTelemetryMaui
        public Task<RouteTelemetryMaui> GetTelemetryAsync();
        public Task<int> SaveTelemetryAsync(RouteTelemetryMaui p);
        public Task<int> DeleteTelemetryAsync(RouteTelemetryMaui p);

        // -- Table LocalisationGroupTraceMaui & LocalisationTraceMaui
        public Task<List<LocalisationTraceMaui>> GetLocalisationsTraceAsync(int id);
        public Task<List<LocalisationGroupTraceMaui>> GetGroupsTraceAsync();
        public Task ClearGroupsTraceAsync();
        public Task<int> SaveAsync(LocalisationGroupDTO p);

    }
}
