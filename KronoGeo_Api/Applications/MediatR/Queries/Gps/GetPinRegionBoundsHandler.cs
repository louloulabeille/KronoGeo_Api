using KronoGeo_Api.Applications.MediatR.Commands.Gps;
using KronoGeo_Api.Infrastructure.Database;
using KronoGeo_Api.Interface.Repository;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Infrastructure.Http;
using KronoGeo_Api.Models.Model.DTO.RegionBounds;
using MediatR;
using Org.BouncyCastle.Asn1.Ocsp;
using Polly;
using System.Net.NetworkInformation;

namespace KronoGeo_Api.Applications.MediatR.Queries.Gps
{
    public class GetPinRegionBoundsHandler(ILogger<GetPinRegionBoundsHandler> logger
        , KronoGeoDbContext context)
        : RepositoryHandler(logger, context)
        , IRequestHandler<GetPinRegionBoundsCommand, ResponseApiPinRegionBounds>
    {
        // -- mise en place d'un système de cluster de Pin
        // -- regroupement

        #region private properties
        /// <summary>
        /// nombre max de retour de pin pour une zone 
        /// </summary>
        private const int MaxClusterPin = 35;
        #endregion

        /// <summary>
        /// retourne le résultat des Cluster Pins pour une région donnée
        /// </summary>
        /// <param name="request"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task<ResponseApiPinRegionBounds> Handle(GetPinRegionBoundsCommand request, CancellationToken cancellationToken)
        {
            double minLat = request.Region.MinBounds.Latitude;
            double minLon = request.Region.MinBounds.Longitude;
            double maxLat = request.Region.MaxBounds.Latitude;
            double maxLon = request.Region.MaxBounds.Longitude;

            var result = await GetPinsInBoundsAsync(minLat, maxLat, minLon, maxLon, request.IdUser, request.Region.IgnoredId);

            if (result.Count > 0)
            {
                
                return new()
                {
                    ApiStatus = EnumApiStatus.Success,
                    PinRegion = result.Take(MaxClusterPin).ToList()
                };
            }

            return new() { ApiStatus = EnumApiStatus.NotFound, Message = "Not found" };
        }

        #region private method
        /// <summary>
        /// Méthod qui retourne les pins dans une région avec les coordonnées 
        /// GPS sans le tri du centre de la zone pour enlever ceux qui sont
        /// le plus loin du centre order by et Take
        /// -- algo fait avec Claude IA
        /// </summary>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <param name="minLon"></param>
        /// <param name="maxLon"></param>
        /// <param name="centerLat"></param>
        /// <param name="centerLon"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public async Task<List<PinDTO>> GetPinsInBoundsAsync(
            double minLat, double maxLat, double minLon, double maxLon
            //double centerLat, double centerLon
            , string idUser, int idIgnore,int max = 50)
        {
            var query = await _unitOfWork.LocalisationSpecifiqueRepository
                .GetLocalisationPhotoByUserIdAndMinMaxLatitude(idUser, minLat, maxLat, idIgnore);

            // Cas du passage de l'antiméridien (±180°)
            // -- pour gérer la frontière entre le + 180 degrès et -180 degrès
            // -- qui se trouve dans le pacifique
            if (minLon <= maxLon)
                query = query.Where(p => p.Longitude >= minLon && p.Longitude <= maxLon);
            else
                query = query.Where(p => p.Longitude >= minLon || p.Longitude <= maxLon);

            return  query
                /*.OrderBy(p => (p.Latitude - centerLat) * (p.Latitude - centerLat)
                            + (p.Longitude - centerLon) * (p.Longitude - centerLon))*/
                .Select(p => new PinDTO()
                {
                    PinCoordonnee = new CoordonneeDTO() { Latitude = p.Latitude, Longitude = p.Longitude },
                    Name = p.Name,
                    Description = p.Description,
                    Path = p.PathPhoto,
                })
                .Take(max)
                .ToList();
        }

        /// <summary>
        /// Method avec filtrage avec le centre de la zone
        /// supprime les points les plus distant du centre
        /// </summary>
        /// <param name="minLat"></param>
        /// <param name="maxLat"></param>
        /// <param name="minLon"></param>
        /// <param name="maxLon"></param>
        /// <param name="centerLat"></param>
        /// <param name="centerLon"></param>
        /// <param name="max"></param>
        /// <returns></returns>
        public async Task<List<PinDTO>> GetPinsInBoundsAsync(
            double minLat, double maxLat, double minLon, double maxLon
            , double centerLat, double centerLon
            , string idUser, int idIgnore
            , int max = 50)
        {
            var query = await _unitOfWork.LocalisationSpecifiqueRepository
                .GetLocalisationPhotoByUserIdAndMinMaxLatitude(idUser, minLat, maxLat, idIgnore);

            // Cas du passage de l'antiméridien (±180°)
            // -- pour gérer la frontière entre le + 180 degrès et -180 degrès
            // -- qui se trouve dans le pacifique
            if (minLon <= maxLon)
                query = query.Where(p => p.Longitude >= minLon && p.Longitude <= maxLon);
            else
                query = query.Where(p => p.Longitude >= minLon || p.Longitude <= maxLon);

            return query
                .OrderBy(p => (p.Latitude - centerLat) * (p.Latitude - centerLat)
                            + (p.Longitude - centerLon) * (p.Longitude - centerLon))
                .Select(p => new PinDTO()
                {
                    PinCoordonnee = new CoordonneeDTO() { Latitude = p.Latitude, Longitude = p.Longitude },
                    Name = p.Name,
                    Description = p.Description,
                    Path = p.PathPhoto,
                })
                .Take(max)
                .ToList();
        }

        #endregion
    }
}
