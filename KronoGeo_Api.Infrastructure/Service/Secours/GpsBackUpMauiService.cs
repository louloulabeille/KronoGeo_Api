using KronoGeo_Api.Infrastructure.Applications.Helpers;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Microsoft.Maui.Devices.Sensors;
using Microsoft.Maui.Storage; // -- attention c'est pour Maui
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace KronoGeo_Api.Infrastructure.Service.Secours
{
    public class GpsBackUpMauiService : IServiceBackupGps
    {
        #region private properties
        private readonly string _pathFile = Path.Combine(FileSystem.AppDataDirectory, IServiceBackupGps.Filename);
        #endregion

        #region public method de l'interface IServiceBackupGps
        public bool FileExist()
        {
            return File.Exists(_pathFile);
        }

        public void DeleteFile()
        {
            if (FileExist())
            {
                File.Delete(_pathFile);
            }
        }

        public List<Localisation>? ReturnLocalisation()
        {
            if (!FileExist()) return null;

            var json = File.ReadAllText(_pathFile);
            if (string.IsNullOrEmpty(json.Trim()))
            {
                DeleteFile(); // -- supprime le fichier si il est vide
                return null;
            }

            var points = JsonSerializer.Deserialize<List<Localisation>>(json, JsonOptions.GetJsonOptions());
            DeleteFile();

            return points;
        }

        public void SavePointsLocalisation(List<Localisation> points)
        {
            if (FileExist()) DeleteFile();

            if (points.Count == 0) return;

            var json = JsonSerializer.Serialize(points);

            if (string.IsNullOrEmpty(json.Trim())) return;
            File.AppendAllText(_pathFile, json);

        }

        #endregion


    }
}
