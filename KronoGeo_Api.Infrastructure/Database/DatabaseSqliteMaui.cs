using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
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
        private readonly SemaphoreSlim _initLock = new(1, 1);
        private bool _initialized = false;
        #endregion

        #region private db sqlite
        private SQLiteAsyncConnection? _db;
        #endregion

        #region IA claude private method
        // -- initialise la base de données sqlite
        private async Task InitAsync()
        {
            // -- si la base existe alors return
            if (_db == null)
            {
                // -- création debase de données sqlite
                var path = Path.Combine(FileSystem.AppDataDirectory, _option.Value.DatabaseName);
                _db = new SQLiteAsyncConnection(path);
            }

            if (_initialized) return;
            await _initLock.WaitAsync();

            try
            {   // -- passage au moins une fois en initialize au démmarage de l'application
                // -- en cas que le model soit modifié marche si le model ajout de colonne
                // -- ou création de la table si inexistant
                await _db.CreateTableAsync<LocalisationMaui>();
                await _db.CreateTableAsync<RouteTelemetryMaui>();
                await _db.CreateTableAsync<LocalisationTraceMaui>();
                await _db.CreateTableAsync<LocalisationGroupTraceMaui>();
                _initialized = true;
            }
            finally { _initLock.Release(); }
        }

        #endregion

        #region public method 
        /// <summary>
        /// Retourne toutes les localisations
        /// </summary>
        /// <returns></returns>
        public async Task<List<LocalisationMaui>> GetAllAsync()
        {
            await InitAsync();
            return await _db!.Table<LocalisationMaui>().ToListAsync();
        }

        /// <summary>
        /// Retourne le nombre d'enregistrement dans la table Localisation
        /// </summary>
        /// <returns></returns>
        public async Task<int> Count()
        {
            await InitAsync();
            return await _db!.Table<LocalisationMaui>().CountAsync();
        }

        /// <summary>
        /// Reset la table des datas de localisation
        /// </summary>
        /// <returns></returns>
        public async Task Clear()
        {
            await InitAsync();
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
            await InitAsync();
            return await _db!.InsertAsync(p);
        }

        /// <summary>
        /// supprime une localisation dans la base de données
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<int> DeleteAsync(LocalisationMaui p)
        {
            await InitAsync();
            return await _db!.DeleteAsync(p);
        }

        /// <summary>
        /// Vérifie si une localisation existe dans la base locale
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<bool> Exists(LocalisationMaui p)
        {
            await InitAsync();
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
            await InitAsync();
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
            await InitAsync();
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
            await InitAsync();
            return await _db!.DeleteAsync(p);
        }

        #endregion

        #region table LocalisationGroupTraceMaui & LocalisationTraceMaui
        /// <summary>
        /// Retourne la liste des localisations pour un id group localisation
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<List<LocalisationTraceMaui>> GetLocalisationsTraceAsync(int id)
        {
            await InitAsync();
            return await _db!.Table<LocalisationTraceMaui>().Where(l => l.LocalisationGroupId == id).ToListAsync();
        }

        /// <summary>
        /// Retourne tous les groups localisations d'un individu
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<List<LocalisationGroupTraceMaui>> GetGroupsTraceAsync()
        {
            await InitAsync();
            return await _db!.Table<LocalisationGroupTraceMaui>().ToListAsync();
        }

        /// <summary>
        /// Supprime les enregistrements des tables LocalisationTraceMaui & LocalisationGroupTraceMaui
        /// </summary>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task ClearGroupsTraceAsync()
        {
            await InitAsync();
            await _db!.DeleteAllAsync<LocalisationTraceMaui>();
            await _db!.DeleteAllAsync<LocalisationGroupTraceMaui>();
        }

        /// <summary>
        /// Enresgistre les éléments dans les tables LocalisationTraceMaui & LocalisationGroupTraceMaui
        /// à partir des LocalisationGroupDTO
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<int> SaveAsync(LocalisationGroupDTO p)
        {
            await InitAsync();
            // -- enregistre le group localisation
            int i = await _db!.InsertAsync(new LocalisationGroupTraceMaui()
            {
                IdGroup = p.Id,
                Name = p.Name,
                Date = p.Date
            });

            if (p.Localisations is not null && p.Localisations.Count > 0)
            {
                foreach (var localisation in p.Localisations)
                {
                    await _db!.InsertAsync(new LocalisationTraceMaui()
                    {
                        IdLocation = localisation.Id,
                        OrderIndex = localisation.OrderIndex,
                        Timestamp = localisation.Timestamp,
                        Latitude = localisation.Latitude,
                        Longitude = localisation.Longitude,
                        Altitude = localisation.Altitude,
                        Accuracy = localisation.Accuracy,
                        VerticalAccuracy = localisation.VerticalAccuracy,
                        Speed = localisation.Speed,
                        Course = localisation.Course,
                        LocalisationGroupId = p.Id
                    });
                    i++;
                }
            }

            return i;
        }
        #endregion
    }
}
