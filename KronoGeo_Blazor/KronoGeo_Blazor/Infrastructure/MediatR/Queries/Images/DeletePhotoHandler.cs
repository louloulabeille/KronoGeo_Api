using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Images;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Images
{
    public class DeletePhotoHandler(ILogger<DeletePhotoHandler> logger
        , IServiceHttpKronoGeo serviceHttpKrono) : ImagesHandler<DeletePhotoHandler>(serviceHttpKrono, logger)
        , IRequestHandler<DeletePhotoCommand, ResponseApiBool>
    {
        public async Task<ResponseApiBool> Handle(DeletePhotoCommand request, CancellationToken cancellationToken)
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
                    return new()
                    {
                        ApiStatus = EnumApiStatus.Problem,
                        Message = "Problème avec la conenxion au serveur.",
                        Retour = false,
                    };
                }

            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Error in DeletePhotoHandler");
                return new()
                {
                    ApiStatus = EnumApiStatus.Problem,
                    Message = "Exception levée, problème interne.",
                    Retour = false
                };
            }
        }
    }
}
