using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
//using static Android.Icu.Text.CaseMap;

namespace KronoGeo_Maui.Applications.Services
{
    public class InMemorySaveLocalisation : IServiceSaveLocalisation
    {

        public async Task<bool> SaveLocalisation( LocalisationGroup localisations , CancellationToken cancellationToken)
        {
            if (localisations.Localisations is null || localisations.Localisations.Count == 0)
                return false;
            else
            return await RequestStoragePermissionsAndSaveFile(localisations , cancellationToken);
        }

        #region private method
        private static async Task<bool> RequestStoragePermissionsAndSaveFile (LocalisationGroup localisations , CancellationToken cancellationToken)
        {
            var readPermissionStatus = await Permissions.RequestAsync<Permissions.StorageRead>();
            var writePermissionStatus = await Permissions.RequestAsync<Permissions.StorageWrite>();

            if (readPermissionStatus != PermissionStatus.Granted ||
                writePermissionStatus != PermissionStatus.Granted)
            {
                await Toast
                    .Make("Les permissions d'enregistrement sont obligatoires pour la sauvegarde du fichier.")
                    .Show(cancellationToken);

                return false;
            }

            return await SaveFile(localisations , cancellationToken);
        }

        private static async Task<bool> SaveFile(LocalisationGroup localisations , CancellationToken cancellationToken) {
            
            /*var guid = Guid.NewGuid();
            var date = DateTime.Today;
            string title = date.ToString("dd-MM-yyyy") + "-" + guid.ToString("N").AsSpan(25).ToString();
*/
            var sb = new StringBuilder();
            sb.AppendLine(" <?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" version=\"1.1\" creator=\"\">");
            sb.AppendLine("<trk>");
            sb.AppendLine($"<name>{localisations.Name}</name>\r\n   <desc/>\r\n   <trkseg>");

            if (localisations.Localisations is null || localisations.Localisations.Count == 0) return false;
            foreach (var localisation in localisations.Localisations)
            {
                sb.AppendLine($"<trkpt lat=\"{localisation.Latitude.ToString(CultureInfo.InvariantCulture)}\" lon=\"{localisation.Longitude.ToString(CultureInfo.InvariantCulture)}\">");
                sb.AppendLine($"<time>{localisation.Timestamp}</time>");
                sb.AppendLine($"<geoidheight>{localisation.Altitude}</geoidheight>");
                sb.AppendLine("</trkpt>");
            }

            sb.AppendLine("</trkseg></trk></gpx>");

            using var stream = new MemoryStream(Encoding.Default.GetBytes(sb.ToString()));

            //var fileSaverResult = await FileSaver.Default.SaveAsync(title+".gpx", stream, cancellationToken);
            var fileSaverResult = await FileSaver.Default.SaveAsync( localisations.Name + ".gpx", stream, cancellationToken);
            if (fileSaverResult.IsSuccessful)
            {
                await Toast.Make($"Le fichier a bien été enregistré : {fileSaverResult.FilePath}").Show(cancellationToken);
                return true;
            }
            else
            {
                await Toast.Make($"Le fichier n'a pas été enregistré avec succès. Erreur: {fileSaverResult.Exception.Message}").Show(cancellationToken);
                return false;
            }
        }
        #endregion
    }

}
