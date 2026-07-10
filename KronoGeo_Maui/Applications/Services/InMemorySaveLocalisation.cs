using CommunityToolkit.Maui.Alerts;
using CommunityToolkit.Maui.Storage;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Npgsql.Internal;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    public class InMemorySaveLocalisation : IServiceSaveLocalisation
    {

        public async Task<bool> SaveLocalisation( List<Localisation> localisations , CancellationToken cancellationToken)
        {
            return await RequestStoragePermissionsAndSaveFile(localisations , cancellationToken);
        }

        #region private method
        private static async Task<bool> RequestStoragePermissionsAndSaveFile (List<Localisation> localisations , CancellationToken cancellationToken)
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

        private static async Task<bool> SaveFile(List<Localisation> localisations , CancellationToken cancellationToken) {
            
            var guid = Guid.NewGuid();
            var date = DateTime.Today;
            string title = date.ToString("dd-MM-yyyy") + "-" + guid.ToString("N").AsSpan(25).ToString();

            var sb = new StringBuilder();
            sb.AppendLine(" <?xml version=\"1.0\" encoding=\"utf-8\"?>");
            sb.AppendLine("<gpx xmlns=\"http://www.topografix.com/GPX/1/1\" version=\"1.1\" creator=\"\">");
            sb.AppendLine("<trk>");
            sb.AppendLine($"<name>{title}</name>\r\n   <desc/>\r\n   <trkseg>");

            foreach (var localisation in localisations)
            {
                sb.AppendLine($"<trkpt lat=\"{localisation.Latitude.ToString(CultureInfo.InvariantCulture)}\" lon=\"{localisation.Longitude.ToString(CultureInfo.InvariantCulture)}\"/>");
            }

            sb.AppendLine("</trkseg></trk></gpx>");

            using var stream = new MemoryStream(Encoding.Default.GetBytes(sb.ToString()));
            
            var fileSaverResult = await FileSaver.Default.SaveAsync(title+".gpx", stream, cancellationToken);
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
