using KronoGeo_Api.Models;
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
    public class DatabaseSqliteMaui
    {
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
            var path = Path.Combine(FileSystem.AppDataDirectory, "app.db3");
            _db = new SQLiteAsyncConnection(path);
            await _db.CreateTableAsync<Localisation>();
        }
        #endregion

        #region public method 
        /// <summary>
        /// Retourne toutes les localisations
        /// </summary>
        /// <returns></returns>
        public async Task<List<Localisation>> GetAllAsync()
        {
            await Init();
            return await _db!.Table<Localisation>().ToListAsync();
        }

        /// <summary>
        /// Retourne le nombre d'enregistrement dans la table Localisation
        /// </summary>
        /// <returns></returns>
        public async Task<int> Count()
        {
            await Init();
            return await _db!.Table<Localisation>().CountAsync();
        }

        /// <summary>
        /// Reset la table des datas de localisation
        /// </summary>
        /// <returns></returns>
        public async Task Reset()
        {
            await Init();
            await _db!.DropTableAsync<Localisation>();
            await _db.CreateTableAsync<Localisation>();
        }

        /// <summary>
        /// Enregistre la localisation dans la base de données
        /// </summary>
        /// <param name="p"></param>
        /// <returns></returns>
        public async Task<int> SaveAsync(Localisation p)
        {
            await Init();
            return await _db!.InsertAsync(p);
        }
        #endregion
    }
}
