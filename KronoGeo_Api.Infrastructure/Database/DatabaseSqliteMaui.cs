using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.Maui;
using Microsoft.Extensions.Options;
using Microsoft.Maui.Storage;
using SQLite;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Api.Infrastructure.Database
{
    /// <summary>
    /// class Database pour l'enregistrement au niveau de Maui
    /// </summary>
    public class DatabaseSqliteMaui (IOptions<SqliteOptions> options) : IServiceDatabaseMaui
    {

        #region private readonly properties
        private readonly IOptions<SqliteOptions> _option = options;
        #endregion

        #region private db sqlite
        private SQLiteAsyncConnection? _db;
        #endregion

        #region IA claude private method
        // -- initialise la base de données sqlite
        private async Task Init()
        {
            // -- si la base existe alors return
            if (_db != null) return;

            // -- création debase de données sqlite
            var path = Path.Combine(FileSystem.AppDataDirectory, _option.Value.DatabaseName);
            _db = new SQLiteAsyncConnection(path);
            await _db.CreateTableAsync<LocalisationMaui>();
            await _db.CreateTableAsync<RouteTelemetryMaui>();
        }
        #endregion

        #region public method 
        /// <summary>
        /// Retourne toutes les localisations
        /// </summary>
        /// <returns></returns>
        public async Task<List<LocalisationMaui>> GetAllAsync()
        {
            await Init();
            return await _db!.Table<LocalisationMaui>().ToListAsync();
        }

        /// <summary>
        /// Retourne le nombre d'enregistrement dans la table Localisation
        /// </summary>
        /// <returns></returns>
        public async Task<int> Count()
        {
            await Init();
            return await _db!.Table<LocalisationMaui>().CountAsync();
        }

        /// <summary>
        /// Reset la table des datas de localisation
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            await Init();
            await _db!.DeleteAllAsync<LocalisationMaui>();
            await _db!.DeleteAllAsync<RouteTelemetryMaui>();
        }

        /// <summary>
        /// Enregistre la localisation dans la base de données
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<int> SaveAsync(LocalisationMaui p)
        {
            await Init();
            return await _db!.InsertAsync(p);
        }

        /// <summary>
        /// supprime une localisation dans la base de données
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(LocalisationMaui p)
        {
            await Init();
            return await _db!.DeleteAsync(p);
        }

        /// <summary>
        /// Vérifie si une localisation existe dans la base locale
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<bool> Exists(LocalisationMaui p)
        {
            await Init();
            return await _db!.Table<LocalisationMaui>()
                .Where(l => l.Longitude == p.Longitude && l.Latitude == p.Latitude).CountAsync() > 0;
        }
        #endregion

        #region public method telemetry
        /// <summary>
        /// Retourne la RouteTelemetry
        /// </summary>
        /// <returns></returns>
        public async Task<RouteTelemetryMaui> GetTelemetryAsync()
        {
            await Init();
            var telemetry = await _db!.Table<RouteTelemetryMaui>().FirstOrDefaultAsync();
            if (telemetry == null)
            {
                telemetry = new RouteTelemetryMaui();
                await _db.InsertAsync(telemetry);
            }
            return telemetry;
        }

        /// <summary>
        /// sauvegarde la Routeteletry
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<int> SaveTelemetryAsync(RouteTelemetryMaui p)
        { 
            await Init();
            // -- supprime tous les enregistrements
            await _db!.DeleteAllAsync<RouteTelemetryMaui>();
            return await _db!.InsertAsync(p); 
        }

        /// <summary>
        /// Supprime la route Telemty
        /// </summary>
        /// <returns></returns>
        public async Task<int> DeleteTelemetryAsync(RouteTelemetryMaui p)
        {
            await Init();
            return await _db!.DeleteAsync(p);
        }

        #endregion
    }
}
