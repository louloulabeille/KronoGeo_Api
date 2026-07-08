using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Repository;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;
using Microsoft.AspNetCore;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class AddPhotoHandler(ILogger<object> logger
        , IWebHostEnvironment webHost) : IRequestHandler<AddPhotoCommand, PhotoDTO>
    {

        #region private fields
        private readonly ILogger<object> _logger = logger;
        private readonly IWebHostEnvironment _webhost = webHost;
        #endregion

        /// <summary>
        /// enregistre les images au niveau du site
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
            string filePath = Path.Combine(_webhost.ContentRootPath, @"images\selfies");

            if (!Directory.Exists(filePath))
            {
                Directory.CreateDirectory(filePath);
            }

            string name = request.FormFile.FileName;
            filePath = Path.Combine(filePath, name);

            using var stream = new FileStream(filePath, FileMode.OpenOrCreate);
            await request.FormFile.CopyToAsync(stream);

            var picture = new PhotoDTO()
            {
                Name        = name,
                PathPhoto   = filePath, 
            };
                
            return picture;
        }
    }
}
