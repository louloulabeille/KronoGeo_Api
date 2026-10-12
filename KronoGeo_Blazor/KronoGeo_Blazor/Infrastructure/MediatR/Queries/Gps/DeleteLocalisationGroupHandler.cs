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
        public async Task<bool> Handle(DeleteLocalisationGroupCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is null)
            {
                Logger.LogError("Le service Http n'existe pas.");
                return false;
            }

            var result = await ServiceHttp.DeleteLocalisationGroupAsync(request.Id);
            if( !result )
            { 
                Logger.LogError("Erreur au niveau de la supression de la localisatin group {id}", request.Id);
            }

            return result;
        }
    }
}
