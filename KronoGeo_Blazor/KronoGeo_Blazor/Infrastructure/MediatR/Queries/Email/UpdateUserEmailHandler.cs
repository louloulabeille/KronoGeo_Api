using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Blazor.Infrastructure.MediatR.Commands.Email;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Queries.Email
{
    public class UpdateUserEmailHandler(ILogger<UpdateUserEmailHandler> logger
        , IServiceHttpManagerKronoGeo serviceHttp)
        : EmailHandler<UpdateUserEmailHandler>(logger, serviceHttp)
        , IRequestHandler<UpdateUserEmailCommand, ResponseApiAuthenticate>
    {
        public async Task<ResponseApiAuthenticate> Handle(UpdateUserEmailCommand request, CancellationToken cancellationToken)
        {
            if (HttpKronoGeo is not null)
            {
                var result = await HttpKronoGeo.UpdateUserEmailAsync(request.RegisterDTO);
                return result;
            }

            return new()
            {
                ApiStatus = EnumApiStatus.Problem,
                Message = "Problème de connexion vers le serveur."
            };
        }
    }
}
