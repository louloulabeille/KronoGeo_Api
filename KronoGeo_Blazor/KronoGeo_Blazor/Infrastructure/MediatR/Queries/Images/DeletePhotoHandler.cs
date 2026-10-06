using KronoGeo_Api.Interface.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Images
{
    public class DeletePhotoHandler(ILogger<DeletePhotoHandler> logger
        , IServiceHttpKronoGeo serviceHttpKrono) : ImagesHandler<DeletePhotoHandler>(serviceHttpKrono, logger)
        , IRequestHandler<DeletePhotoCommand, bool>
    {
        public async Task<bool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
        {
            try
            {
                if (ServiceHttp is not null)
                {
                    return await ServiceHttp.DeleteImageAsync(request.Id);
                }
                else
                {
                    Logger.LogError("ServiceHttp is null in DeletePhotoHandler");
                    return false;
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in DeletePhotoHandler");
                return false;
            }
        }
    }
}
