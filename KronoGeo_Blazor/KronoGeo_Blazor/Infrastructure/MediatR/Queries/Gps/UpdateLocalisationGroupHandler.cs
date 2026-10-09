using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Gps;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Gps
{
    public class UpdateLocalisationGroupHandler
        : GpsHandler<UpdateLocalisationGroupHandler>
        , IRequestHandler<UpdateLocalisationGroupCommand, ResponseApiLocalisations>
    {
        public Task<ResponseApiLocalisations> Handle(UpdateLocalisationGroupCommand request, CancellationToken cancellationToken)
        {
            if (ServiceHttp is not null)
            {
                // -- requete vers l'APi
                var result = await ServiceHttp();

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
