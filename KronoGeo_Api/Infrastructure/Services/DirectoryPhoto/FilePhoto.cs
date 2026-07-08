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
        , IWebHostEnvironment webHost) : IServiceGestionPhoto
    {
        #region private readonly properties
        private readonly IOptions<PhotoOptions> _option = option;
        private readonly IWebHostEnvironment _webhost = webHost;
        private readonly CancellationToken _cancellation = new();
        #endregion

        #region public method
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


        #endregion

    }
}
