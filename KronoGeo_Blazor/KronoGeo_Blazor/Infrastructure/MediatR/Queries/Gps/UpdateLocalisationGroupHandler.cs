using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class UpdateLocalisationGroupHandler 
        (IServiceHttpKronoGeo serviceHttp , ILogger<UpdateLocalisationGroupHandler> logger)
        : GpsHandler<UpdateLocalisationGroupHandler>(serviceHttp, logger)
        , IRequestHandler<UpdateLocalisationGroupCommand, ResponseApiLocalisations>
    {
        public async Task<ResponseApiLocalisations> Handle(UpdateLocalisationGroupCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is not null)
            {
                // -- requete vers l'APi
                var result = await ServiceHttp.UpdateLocalisationGroupAsync(request.LocalisationGroup);

                if (result is not null)
                {
                    return result;
                }
            }

            Logger.LogError(request.LocalisationGroup.ToString(),"Erreur lors de l'enregistrement.");
            return new ResponseApiLocalisations
            {
                ApiStatus = EnumApiStatus.BadRequest,
                Message = "Erreur lors de l'enregistrement de la localisation group."
            };
        }
    }
}
