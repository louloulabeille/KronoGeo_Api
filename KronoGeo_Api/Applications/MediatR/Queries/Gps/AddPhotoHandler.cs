using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Repository;
using KronoGeo_Api.Models.Infrastructure.Options;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore;
using Microsoft.Extensions.Options;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class AddPhotoHandler(ILogger<object> logger
        , IWebHostEnvironment webHost
        , IOptions<PhotoOptions> options) : IRequestHandler<AddPhotoCommand, PhotoDTO>
    {

        #region private fields
        private readonly ILogger<object> _logger = logger;
        private readonly IWebHostEnvironment _webhost = webHost;
        private readonly IOptions<PhotoOptions> _options = options;
        private readonly CancellationToken _cancellation = new();
        #endregion

        /// <summary>
        /// enregistre les images au niveau du site dans un répertoire temporaire
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public async Task<PhotoDTO> Handle(AddPhotoCommand request, CancellationToken cancellationToken)
        {
            /* lire le flux de données de la requete
           using var stream = new StreamReader(Request.Body);
           var result = await stream.ReadToEndAsync();
           */
            //string filePath = Path.Combine(_webhost.ContentRootPath, @"images\tmp");
            string filePath = Path.Combine(_webhost.ContentRootPath, @$"{_options.Value.Tmp_Photo}");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string name = request.FormFile.FileName;
            filePath = Path.Combine(filePath, name);

            using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            await request.FormFile.CopyToAsync(stream, _cancellation );

            var picture = new PhotoDTO()
            {
                Name        = name,
                PathPhoto = @$"{_options.Value.Tmp_Photo}", 
            };
                
            return picture;
        }
    }
}
