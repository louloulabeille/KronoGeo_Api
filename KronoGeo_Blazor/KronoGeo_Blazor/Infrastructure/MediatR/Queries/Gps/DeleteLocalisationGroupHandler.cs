using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class DeleteLocalisationGroupHandler(IServiceHttpKronoGeo serviceHttp
        , ILogger<DeleteLocalisationGroupHandler> logger)
        : GpsHandler<DeleteLocalisationGroupHandler>(serviceHttp, logger)
        , IRequestHandler<DeleteLocalisationGroupCommand, ResponseApiBool>
    {
        public async Task<ResponseApiBool> Handle(DeleteLocalisationGroupCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is null)
            {
                Logger.LogError("Le service Http n'existe pas.");
                return new() { ApiStatus = EnumApiStatus.Problem, 
                    Message = "Problème avec la connexion au serveur.",
                    Retour = false
                };
            }

            var result = await ServiceHttp.DeleteLocalisationGroupAsync(request.Id);
            if( !result.Retour )
            { 
                Logger.LogError("Erreur au niveau de la supression de la localisatin group {id}", request.Id);
            }

            return result;
        }
    }
}
