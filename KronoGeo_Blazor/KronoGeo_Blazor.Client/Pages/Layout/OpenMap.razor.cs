using BruTile.Predefined;
using BruTile.Web;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Mapsui;
using Mapsui.Extensions;
using Mapsui.Layers;
using Mapsui.Manipulations;
using Mapsui.Nts;
using Mapsui.Projections;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.Tiling.Layers;
using Mapsui.UI;
using Mapsui.UI.Blazor;
using Mapsui.Utilities;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using NetTopologySuite.Geometries;
using SkiaSharp;
using Svg.Skia;
using static System.Net.WebRequestMethods;

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
        
        // ------- card image
        protected bool IsHovered { get; set; } = false; // -- affichage de l'image dans une card image boostrap
        protected string UrlImg { get; set; }  = string.Empty; // -- url de l'image à afficher
        protected double MouseY { get; set; } = 0;
        protected double MouseX { get; set; } = 0;
        protected string ImgDate { get; set; } = string.Empty;
        protected string ImgLongitude { get;set; } = string.Empty;
        protected string ImgLatitude { get; set; } = string.Empty;
        // -------
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
                //string userAgent = "Kronogeo/1.0 (louloulabeille@alwaysdata.net)";
                //MapControl?.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer(userAgent));
                //MapControl?.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer(null));


                // -- Création de la source de tuiles OpenStreeMap
                 
                var cartoDbTileSource = new HttpTileSource(
                    new GlobalSphericalMercator(),
                    "https://localhost:7186/tiles/{z}/{x}/{y}.png",
                    name: "Open Street Map"
                );

                // 2. Création de la couche Mapsui
                var tileLayer = new TileLayer(cartoDbTileSource)
                {
                    Name = "OpenStreetMap"
                };

                //// 3. Ajout à la carte
                MapControl?.Map?.Layers.Add(tileLayer);

            }
        }

        protected override void OnInitialized()
        {
            MapStateService?.OnOpenRequested += HandleOpenRequested;
            base.OnInitialized();
        }

        #endregion

        #region protected method 
        /// <summary>
        /// method pour event onpointermove
        /// </summary>
        /// <param name="e"></param>
        protected void HandlerPointerMove(PointerEventArgs e)
        {
            if (MapControl?.Map == null) return;

            // Conversion des coordonnées écran vers la carte Mapsui
            //var viewport = MapControl.Map.Navigator.Viewport;
            //var worldPoint = viewport.ScreenToWorld(e.OffsetX, e.OffsetY);

            // -- récupération des calques de la map pour avoir les objects qui sont affichés dessus
            var layers = MapControl?.Map?.Layers;
            if (layers is null) return;

            // Détection si le pointeur survole un point (HitTesting)
            var mapInfo = MapControl?.GetMapInfo(new ScreenPosition(e.OffsetX, e.OffsetY), layers);

            // recherche si un feature existe sur la map && qu'il existe dans le dictionnaire
            if (mapInfo?.Feature != null && _featureImageMap.TryGetValue(mapInfo.Feature, out var imgUrl))
            {
                UrlImg = Path.Combine("https://localhost:7291/" + imgUrl.PathPhoto?.Replace("wwwroot/", "") , imgUrl.Name) ;
                MouseX = e.OffsetX;
                MouseY = e.OffsetY;
                IsHovered = true;

                ImgDate = imgUrl.Timestamp.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                ImgLongitude = imgUrl.Longitude.ToString();
                ImgLatitude = imgUrl.Latitude.ToString();
            }
            //else
            //{
            //    MouseX = 0;
            //    MouseY = 0;
            //    IsHovered = false;
            //}

            StateHasChanged();
        }

        /// <summary>
        /// ferme la card image
        /// </summary>
        protected void HandlerCloseCard()
        {
            MouseX = 0;
            MouseY = 0;
            IsHovered = false;
            UrlImg = string.Empty;

            ImgDate = string.Empty;
            ImgLongitude = string.Empty;
            ImgLatitude = string.Empty;

            StateHasChanged();
        }
        #endregion


        #region private methods
        /// <summary>
        /// method qui est appelé lors de la transmission des localisations
        /// </summary>
        private  void HandleOpenRequested()
        {
            if( MapStateService?.CurrentLocalisations is not null && MapStateService?.CurrentLocalisations?.Count() > 1)
            { // -- affichage des localisations
                InitMap();
                HandlerCloseCard();
                ChargingTraceAndPoint(MapStateService.CurrentLocalisations);
                StateHasChanged();
            }
            else if( MapStateService?.CurrentLocalisations is not null && MapStateService?.CurrentLocalisations?.Count() == 1)
            { // -- affichage poun un point
                InitMap();
                HandlerCloseCard();
                ZoomTo(MapStateService?.CurrentLocalisations?.First(), 18);
                StateHasChanged();
            }
            else
            {   // -- initialise tout
                InitMap();
                HandlerCloseCard();
                StateHasChanged();
            }
        }

        /// <summary>
        /// method qui charge les tracés et les points sur la map
        /// avec l'aide de layer
        /// </summary>
        /// <param name="localisations"></param>
        private void ChargingTraceAndPoint( IEnumerable<Localisation> localisations )
        {
            if (MapControl is null || !localisations.Any()) return;

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
        /// Affiche les points layer pour les photos
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        private MemoryLayer? AddPointImage( IEnumerable<Localisation> localisations ) {

            var photos = localisations.OrderBy(x => x.OrderIndex).OfType<LocalisationPhoto>().ToList();

            if (photos is null || photos.Count == 0 ) return default;
            var features = new List<Mapsui.IFeature>();

            foreach (var photo in photos) {

                if ( string.IsNullOrEmpty(photo.PathPhoto)) continue;

                // -- transformation des points longitude et latitude en points mercator
                var coordonate = SphericalMercator.FromLonLat(photo.Longitude, photo.Latitude);
                var feature = new PointFeature(coordonate);

                feature.Styles.Add(ImgPin());

                // -- ajout dans le dictionnaire pour retrouver les photos par rapport 
                // -- à leur feature
                _featureImageMap.Add(feature, photo);

                // -- ajout dans la liste des features pour l'intégrer dans le layer
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
        /// créer l'object qui va afficher les lignes au niveau des points de géolocalisation
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
        private void ZoomTo (Localisation? localisation, double zoom = 15 )
        {
            if (localisation is null) return;

            // Get the lon lat coordinates from somewhere (Mapsui can not help you there)
            var center = new MPoint(localisation.Longitude, localisation.Latitude);
            // OSM uses spherical mercator coordinates. So transform the lon lat coordinates to spherical mercator
            var sphericalMercatorCoordinate = SphericalMercator.FromLonLat(center.X, center.Y).ToMPoint();
            // Set the center of the viewport to the coordinate. The UI will refresh automatically
            // Additionally you might want to set the resolution, this could depend on your specific purpose
            MapControl?.Map.Navigator.CenterOnAndZoomTo(sphericalMercatorCoordinate, zoom);
            MapControl?.Refresh();
        }

        /// <summary>
        /// fait un zoom en prenant les 4 points d'un rectangle min et max d'un tracé
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
            MapControl?.Refresh();
        }

        /// <summary>
        /// inisialise la map en suprimant le tracé & les points photos si déjà affiché
        /// </summary>
        private void InitMap ()
        {
            if (_trace is not null)
                MapControl?.Map?.Layers.Remove(_trace);

            if (_photoLayer is not null)
                MapControl?.Map?.Layers.Remove(_photoLayer);
        }

        /// <summary>
        /// Retourne le style image pour qu'il soit afficher
        /// </summary>
        /// <returns></returns>
        private static ImageStyle ImgPin()
        {
            var svg = $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-camera2\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M5 8c0-1.657 2.343-3 4-3V4a4 4 0 0 0-4 4\"/>\r\n  <path d=\"M12.318 3h2.015C15.253 3 16 3.746 16 4.667v6.666c0 .92-.746 1.667-1.667 1.667h-2.015A5.97 5.97 0 0 1 9 14a5.97 5.97 0 0 1-3.318-1H1.667C.747 13 0 12.254 0 11.333V4.667C0 3.747.746 3 1.667 3H2a1 1 0 0 1 1-1h1a1 1 0 0 1 1 1h.682A5.97 5.97 0 0 1 9 2c1.227 0 2.367.368 3.318 1M2 4.5a.5.5 0 1 0-1 0 .5.5 0 0 0 1 0M14 8A5 5 0 1 0 4 8a5 5 0 0 0 10 0\"/>\r\n</svg>";
            var imageStyle = new ImageStyle
            {
                Image = new Image { Source = $"svg-content://{svg}" },
                SymbolScale = 1,
            };

            return imageStyle;
        }

        public void InitMapAndZoom ()
        {
            InitMap();

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
