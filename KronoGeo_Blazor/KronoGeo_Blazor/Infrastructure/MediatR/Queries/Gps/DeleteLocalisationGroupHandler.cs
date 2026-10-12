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
            throw new NotImplementedException();
        }
    }
}
