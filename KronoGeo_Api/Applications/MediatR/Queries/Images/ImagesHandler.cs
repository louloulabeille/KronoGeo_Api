using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Infrastructure.UnitOfWork;
using Polly;
using Serilog.Core;

namespace KronoGeo_Api.Applications.MediatR.Queries.Images
{
    public class ImagesHandler(ILogger<object> logger , KronoGeoDbContext context)
    {
        #region internal properties
        internal readonly ILogger<object> _logger = logger;
        internal readonly UnitOfWork _unitOfWork = new(context);
        #endregion
    }
}
