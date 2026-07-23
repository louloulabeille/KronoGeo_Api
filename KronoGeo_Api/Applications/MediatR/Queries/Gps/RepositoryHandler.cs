using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Infrastructure.UnitOfWork;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class RepositoryHandler (ILogger<object> logger
        , KronoGeoDbContext context)
{
        #region internal properties
        internal readonly ILogger<object> _logger = logger;
        internal readonly UnitOfWork _unitOfWork = new(context);
        #endregion
    }
}
