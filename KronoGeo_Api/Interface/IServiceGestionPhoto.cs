using KronoGeo_Api.Models.Model.DTO;

namespace KronoGeo_Api.Interface
{
    public interface IServiceGestionPhoto
    {
        public Task<PhotoDTO> SavePhotoHttp(IFormFile formFile);
        public Task<PhotoDTO?> CutPhoto(string directory , PhotoDTO photo );
    }
}
