using KronoGeo_Api.Interface;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using Microsoft.Extensions.Options;
using System.IO;

namespace KronoGeo_Api.Infrastructure.Services.DirectoryPhoto
{
    /// <summary>
    /// classe de  gestion au niveau 
    /// </summary>
    public class FilePhoto(IOptions<PhotoOptions> option
        , IWebHostEnvironment webHost, ILogger<FilePhoto> logger) : IServiceGestionPhoto
    {
        #region private readonly properties
        private readonly IOptions<PhotoOptions> _option = option;
        private readonly IWebHostEnvironment _webhost = webHost;
        private readonly CancellationToken _cancellation = new();
        private readonly ILogger<FilePhoto> _logger = logger;
        #endregion

        #region public method interface
        /// <summary>
        /// récupère le flux d'une image au niveau du controleur pourl'enregister au niveau du serveur
        /// </summary>
        /// <param name="formFile"></param>
        /// <returns></returns>
        public async Task<PhotoDTO> SavePhotoHttp(IFormFile formFile)
        {
            string filePath = Path.Combine(_webhost.ContentRootPath, @$"{_option.Value.Tmp_Photo}");


            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string name = formFile.FileName;
            filePath = Path.Combine(filePath, name);

            using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            await formFile.CopyToAsync(stream, _cancellation);

            var picture = new PhotoDTO()
            {
                Name = name,
                PathPhoto = @$"{_option.Value.Tmp_Photo}",
            };

            return picture;
        }

        /// <summary>
        /// déplace une image vers son répertoire de stockage
        /// </summary>
        /// <param name="directory">répertoire final de copie</param>
        /// <param name="photo">photo a déplacé</param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PhotoDTO?> CutPhoto(string directory, PhotoDTO photo)
        {
            if (!string.IsNullOrEmpty(photo.Name) || !string.IsNullOrEmpty(photo.PathPhoto))
            {
                // -- répertoire de copie du fichier
                string directoryPathCopy = Path.Combine(_webhost.ContentRootPath, @$"{_option.Value.Default_Photo}");
                directoryPathCopy = Path.Combine(directoryPathCopy, @$"{directory}");
                // -- répertoire temporaire des images
                string directoryPath = Path.Combine(_webhost.ContentRootPath, @$"{_option.Value.Tmp_Photo}"); // -- répertoire temporaire
                var photoDest = new PhotoDTO() { Name = photo.Name, PathPhoto = directoryPathCopy };

                if (!Directory.Exists(directoryPath))
                {
                    return null;
                }

                if ( !Directory.Exists(directoryPathCopy))
                {
                    Directory.CreateDirectory(directoryPathCopy);
                }

                directoryPathCopy = Path.Combine(directoryPathCopy, photo.Name ?? string.Empty);
                directoryPath = Path.Combine(directoryPath, photo.Name ?? string.Empty);
                try
                {
                    File.Copy(directoryPath, directoryPathCopy );
                    File.Delete(directoryPath); // -- suppression du fichier dans le répetoire temporaire
                    return photoDest;
                }
                catch(IOException copyError)
                {
                    _logger.LogError(copyError, "Erreur de copie de l'image {Source} répertoire {Dest}", directoryPathCopy, directoryPath);
                    return null;
                }
            }

            return null;
        }

        /// <summary>
        /// methode pour supprimer le répertoire (si vide) et la photo
        /// </summary>
        /// <param name="photo"></param>
        public void DeletePhotos(string directory)
        {
            if (string.IsNullOrEmpty(directory)) return;

            string directoryPath = Path.Combine(_webhost.ContentRootPath, directory);
            if (!Directory.Exists(directoryPath)) return;

            string[] fileList = Directory.GetFiles(directoryPath);
            foreach (string file in fileList)
            {
                File.Delete(file);
            }

            Directory.Delete(directoryPath);

        }
        #endregion

    }
}
