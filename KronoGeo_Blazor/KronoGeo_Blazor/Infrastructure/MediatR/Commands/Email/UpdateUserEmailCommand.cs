using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Blazor.Infrastructure.MediatR.Commands.Email
{
    public class UpdateUserEmailCommand : IRequest<ResponseApiAuthenticate>
    {
        public required ProfilUpdateEmailDTO RegisterDTO { get; set; }
    }
}
