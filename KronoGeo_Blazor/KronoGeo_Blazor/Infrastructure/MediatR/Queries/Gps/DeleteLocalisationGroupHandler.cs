using KronoGeo_Api.Interface.Service;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class DeleteLocalisationGroupHandler(IServiceHttpKronoGeo serviceHttp
        , ILogger<DeleteLocalisationGroupHandler> logger)
        : GpsHandler<DeleteLocalisationGroupHandler>(serviceHttp, logger)
        , IRequestHandler<DeleteLocalisationGroupCommand, bool>
    {
        public Task<bool> Handle(DeleteLocalisationGroupCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is null)
            {
                Logger.LogError("Le service Http n'existe pas.")
                return false;
            }

            using var result = ServiceHttp.DeleteLocalisationGroupAsync(request.Id);
            if (result is not null)
                return result;

            Logger.LogError("Erreur au niveau de la supression de la localisatin group {id}", request.Id);
            return false;
        }
    }
}
