using KronoGeo_Api.Interface.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Images
{
    public class UpdatePhotoHandler(IServiceHttpKronoGeo serviceHttp
        , ILogger<UpdatePhotoHandler> logger) : ImagesHandler<UpdatePhotoHandler>(serviceHttp, logger)
        , IRequestHandler<UpdatePhotoCommand, bool>

    {
        public async Task<bool> Handle(UpdatePhotoCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is not null)
            {
                return await ServiceHttp.UpdateImageAsync( request.Photo );
            }

            return false;
        }
    }
}
