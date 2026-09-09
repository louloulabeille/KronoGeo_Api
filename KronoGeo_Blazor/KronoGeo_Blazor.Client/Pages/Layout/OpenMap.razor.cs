using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI.Blazor;
using Mapsui.Utilities;
using Microsoft.AspNetCore.Components;
using NetTopologySuite.Features;
using NetTopologySuite.Geometries;
using Org.BouncyCastle.Bcpg.Sig;
using System.Net.Sockets;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class OpenMapBase : ComponentBase, IDisposable
    {
        #region public inject
        [Inject]
        public IMapStateService? MapStateService { get; set; }
        #endregion

        #region public properties
        [Parameter]
        public Localisation? InitLocalisation { get; set; }
        #endregion

        #region protected properties
        protected MapControl? MapControl;
        protected List<Localisation>? Localisations { get; set; }
        #endregion

        #region private properties
        private Layer? _trace = default;
        private MemoryLayer? _photoLayer = default; 
        private readonly Dictionary<Mapsui.IFeature, LocalisationPhoto> _featureImageMap = [];
        #endregion

        #region protected override method
        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                MapControl?.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            }
        }

        protected override void OnInitialized()
        {
            MapStateService?.OnOpenRequested += HandleOpenRequested;
            base.OnInitialized();
        }

        #endregion

        #region private methods
        /// <summary>
        /// method qui est appelé lors de la transmission des localisations
        /// </summary>
        private void HandleOpenRequested()
        {
            if( MapStateService?.CurrentLocalisations is not null || MapStateService?.CurrentLocalisations?.Count() > 0)
            {
                ChargingTraceAndPoint(MapStateService.CurrentLocalisations);
                InvokeAsync(StateHasChanged);
            }
        }

        /// <summary>
        /// method qui charge les tracés ou les points sur la map
        /// avec l'aide de layer
        /// </summary>
        /// <param name="localisations"></param>
        private void ChargingTraceAndPoint( IEnumerable<Localisation> localisations )
        {
            if (MapControl is null || !localisations.Any()) return;

            InitMap();

            // -- chargement des tracés
            var traceLayer = AddTraceLocalisation(localisations, out GeometryFeature outLine);
            // -- ajout du tracé au niveau des calques au niveau du conteneur
            MapControl.Map.Layers.Add(traceLayer);

            // -- chargement des points
            var pointLayer = AddPointImage(localisations);
            if(pointLayer is not null)
                MapControl.Map.Layers.Add(pointLayer);

            ZoomToBox(outLine.Geometry?.EnvelopeInternal);
            //ZoomTo(initMap, 22);

            MapControl.Refresh();

        }

        /// <summary>
        /// Affiche le layer pour les photos
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        private MemoryLayer? AddPointImage( IEnumerable<Localisation> localisations ) {
            var photos = localisations.OrderBy(x => x.OrderIndex).OfType<LocalisationPhoto>().ToList();

            if (photos is null || photos.Count == 0 ) return default;
            var features = new List<Mapsui.IFeature>();

            foreach (var photo in photos) {

                if (photo.PathPhoto is null) continue;

                var coordonate = SphericalMercator.FromLonLat(photo.Longitude, photo.Latitude);
                var feature = new PointFeature(coordonate);

                feature.Styles.Add(new SymbolStyle
                {
                    SymbolScale = 0.2,
                    Fill = new Brush(Color.DeepPink),
                    Outline = new Pen(Color.MintCream)
                });

                _featureImageMap.Add(feature, photo);
                features.Add(feature);
            }

            var memoryLayer = new MemoryLayer
            {
                Name = "Photo",
                Features = features
            };

            _photoLayer = memoryLayer;

            return memoryLayer;


        }

        /// <summary>
        /// créer l'object qui va être pris dans le calque pour afficher les lignes au niveau des points de géolocalisation
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        private Layer AddTraceLocalisation( IEnumerable<Localisation> localisations, out GeometryFeature outLine) {
            
            // -- tracé par défaut affiché 
            _trace = default;

            // -- création des coordonnées
            var coordinates = localisations.OrderBy(x => x.OrderIndex)
                .Select(p => new Coordinate(p.Longitude, p.Latitude))
                .ToArray();

            // -- prise en premier des premières coordonnées pour pouvoir zoomer dessus
            var initMap = localisations.OrderBy(x => x.OrderIndex).FirstOrDefault();

            // -- création de l'oject Linestring  qui va faire le tracé
            var lineGeometry = new LineString(coordinates);

            // -- création de IFeature object geometry
            outLine = new GeometryFeature()
            {
                Geometry = lineGeometry, // -- coordonneés
                Styles =    // -- style pour l'affichage
                [ new VectorStyle
                    {
                        Line = new Pen
                        {
                            Color = Color.RoyalBlue ,  // Couleur du tracé
                            Width = 2                   // Épaisseur de la ligne
                        },
                        //Outline = new Pen(Color.Violet, width: 5)
                    }
                 ]
            };

            // -- création du memoryProvider 
            var memoryProvider = new MemoryProvider(outLine)
            {
                // -- système de référence par latitude et longitude 
                // -- voir ce lien https://www.sigterritoires.fr/index.php/epsg-4326-vs-3857/
                CRS = "EPSG:4326" // The DataSource CRS needs to be set
            };

            // -- création des datas pour que la carte OpenStreetMap puisse les lire
            // -- il prend en compte les points Mercator c'est à dire EPSG:3857
            // -- il faut le mentionner pour qu'il fasse la transformation des points de géolocalisation
            var dataSource = new ProjectingProvider(memoryProvider)
            {
                CRS = "EPSG:3857"
            };

            // -- création du Layer pour intéger sur la map au niveau du calque
            var traceLayer = new Layer()
            {
                DataSource = dataSource,
                Name = "Tracé"
            };

            _trace = traceLayer;

            return traceLayer;
        }

        /// <summary>
        /// fait un zoom sur un point de géolocation
        /// </summary>
        /// <param name="localistion"></param>
        /// <param name="zoom"></param>
        private void ZoomTo (Localisation? localistion, double zoom )
        {
            if (localistion is null) return;

            // Get the lon lat coordinates from somewhere (Mapsui can not help you there)
            var center = new MPoint(localistion.Longitude, localistion.Latitude);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(center.X, center.Y).ToMPoint();
            // Set the center of the viewport to the coordinate. The UI will refresh automatically
            // Additionally you might want to set the resolution, this could depend on your specific purpose
            MapControl?.Map.Navigator.CenterOnAndZoomTo(sphericalMercatorCoordinate, zoom);
        }

        /// <summary>
        /// fait un zoom en prenant les 4 points min et max d'un tracé
        /// qu'il appelle envelope
        /// </summary>
        /// <param name="env"></param>
        private void ZoomToBox(Envelope? env)
        {
            if (env is null) return;

            var minPoint = SphericalMercator.FromLonLat(env.MinX, env.MinY);
            var maxPoint = SphericalMercator.FromLonLat(env.MaxX, env.MaxY);

            var box = new MRect(minPoint.x, minPoint.y, maxPoint.x, maxPoint.y);

            MapControl?.Map.Navigator.ZoomToBox(box, duration: 500);
        }

        /// <summary>
        /// inisialise la map en suprimant le tracé si déjà affiché
        /// </summary>
        private void InitMap ()
        {
            if (_trace is not null)
                MapControl?.Map?.Layers.Remove(_trace);

            if (_photoLayer is not null)
                MapControl?.Map?.Layers.Remove(_photoLayer);
        }
        #endregion

            #region public method interface Idisposable
        public void Dispose()
        {
            MapStateService?.OnOpenRequested -= HandleOpenRequested;
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
