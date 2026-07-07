using KronoGeo_Api.Models.Model.DTO;
using MediatR;

namespace KronoGeo_Api.Applications.MediatR.Commands.Gps
{
    public class AddLocalisationsCommand : IRequest<LocalisationGroupDTO>
    { 
        public required LocalisationGroupDTO LocalisationGroup { get; set; }
    }
}
