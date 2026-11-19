using BruTile.Predefined;
using BruTile.Web;
using DotNext.Collections.Generic;
using KronoGeo_Api.Infrastructure.Service.Map;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Api.Models.Model.DTO;
using KronoGeo_Api.Models.Model.DTO.RegionBounds;
using KronoGeo_Api.Models.RegionBounds;
using KronoGeo_Blazor.Client.Infrastructure.Service;
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
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.Web;
using NetTopologySuite.Geometries;
using SkiaSharp;
using Svg.Skia;
using System.ComponentModel;
using System.Security.Claims;
using static Microsoft.Maui.ApplicationModel.Permissions;
using static System.Net.WebRequestMethods;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class OpenMapBase : ComponentBase, IDisposable
    {
        #region private const properties
        // -- temps de recherche et d'affichage des pins sur la map
        private const int tpsTempo = 1800;
        #endregion

        #region public inject
        [Inject]
        public IMapStateService? MapStateService { get; set; }
        [Inject]
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;
        [Inject]
        private AuthenticationStateProvider? _authenticationStateProvider { get; set; } = default;
        /// <summary>
        /// Service pour l'affichage des notifications toast
        /// </summary>
        [Inject]
        private ToastsService? _toastService { get; set; } = default;
        [Inject]
        private NavigationManager? _navigationManager { get; set; } = default;
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
        protected LocalisationPhoto? LocalisationPhoto { get; set; }
        protected string Description { get; set; } = string.Empty;
        protected bool IsDescription { get; set; } = false;
        // -------
        #endregion

        #region private properties
        private Layer? _trace = default;
        private MemoryLayer? _photoLayer = default;
        private MemoryLayer? _pinLayer = default;
        private MemoryLayer? _clusterLayer = default;
        // -- dictionnaire des images qui sont affichés de la map ( -- images du tracé en cours --)
        private readonly Dictionary<Mapsui.IFeature, LocalisationPhoto> _featureImageMap = [];
        // -- dictionnaire des pins qui sont affichés et qui appartiennent aux autres tracés
        private readonly Dictionary<Mapsui.IFeature, Pin> _featurePinMap = [];
        // -- dictionnaire des clusters qui sont affichés à la place des Pins (pour réunir les pins avec le même point)
        private readonly Dictionary<Mapsui.IFeature, ClusterPin> _featureClusterMap = [];
        private Mapsui.IFeature? _viewPhoto = default;
        private CancellationTokenSource? _cts;
        
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
            // -- event pour afficher les cards au niveau du survol
            MapStateService?.OnOpenRequested += HandleOpenRequested;
            
            base.OnInitialized();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if( firstRender )
            {
                // -- initialisation de la Map avec un event qui va se déclencher
                // -- quand la Map va bouger après 400 ms pour éviter de tout rechercher à la voler
                if (MapControl is not null)
                    InitMap(MapControl.Map);
            }

            return base.OnAfterRenderAsync(firstRender);
        }
        #endregion

        #region protected method 
        /// <summary>
        /// method pour event onpointermove pour afficher la card de la photo
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
                // -- feature qui est survolé par la souris
                _viewPhoto = mapInfo?.Feature;

                // -- fermeture de la saisie de description
                IsDescription = false;

                UrlImg = Path.Combine("https://localhost:7291/" + imgUrl.PathPhoto?.Replace("wwwroot/", "") , imgUrl.Name) ;
                MouseX = e.OffsetX;
                MouseY = e.OffsetY;
                IsHovered = true;

                ImgDate = imgUrl.Timestamp.LocalDateTime.ToString("dd/MM/yyyy HH:mm:ss");
                ImgLongitude = imgUrl.Longitude.ToString();
                ImgLatitude = imgUrl.Latitude.ToString();
                LocalisationPhoto = imgUrl;
                Description = imgUrl.Description ?? string.Empty;
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
            LocalisationPhoto = null;
            _viewPhoto = null;

            StateHasChanged();
        }

        /// <summary>
        /// Affiche le textAera et l'enregistre si modifié
        /// Modification et ajout d'une description sur la photo
        /// </summary>
        /// <param name="photo"></param>
        protected async Task EditDescription ()
        {
            try
            {
                IsDescription = !IsDescription;

                if (!IsDescription && !string.IsNullOrEmpty(Description.Trim()) 
                    && Description.Trim() != LocalisationPhoto?.Description)
                { // -- code d'enregistrement de la description 

                    if (_authenticationStateProvider is not null && LocalisationPhoto is not null && _serviceHttp is not null)
                    {
                        var initText = LocalisationPhoto.Description;
                        LocalisationPhoto.Description = Description.Trim();
                        
                        var localisationPhotoDTO = LocalisationPhoto.GetDTO() as LocalisationPhotoDTO ?? null;

                        if (localisationPhotoDTO is null) return;
                        var result = await _serviceHttp.UpdateImageAsync(localisationPhotoDTO);
                        if ( result.Retour  && _toastService is not null)
                        {
                            // -- affichage d'un message de succès
                            await _toastService.SuccesAsync($"Description modifiée avec succès.");
                            StateHasChanged();
                        }
                        else if (result.IsUnauthorized)
                        {   // -- logout si unauthorized
                            _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                        }
                        else 
                        {
                            LocalisationPhoto.Description = initText;
                            if (_toastService is not  null)
                                // -- affichage d'un message de succès
                                await _toastService.SuccesAsync($"Impossible de faire la modification.");
                        }
                    }

                }
                StateHasChanged();
            }
            catch(Exception ex)
            {
                Console.WriteLine($"Message error OpenMap method EditDescription : {ex.Message}");
                // -- affichage d"un message Toasts d'erreur de boostrap fait en javascript
                if (_toastService is not null)
                    await _toastService.ErreurAsync($"Erreur lors de la modification de la description.");
            }
        }

        /// <summary>
        /// Supprime la photo de la localisation et du serveur
        /// </summary>
        /// <returns></returns>
        protected async Task DeletePhoto()
        {
            try
            {
                var photo = LocalisationPhoto?.GetDTO();
                if (photo is not null && _serviceHttp is not null)
                {
                    var result = await _serviceHttp.DeleteImageAsync(photo);
                    if (result.IsSuccess && result.Retour && _viewPhoto is not null 
                        && LocalisationPhoto is not null)
                    {
                        // -- affichage d'un message de succès
                        if(_toastService is not null)
                            await _toastService.SuccesAsync($"Photo supprimée avec succès.");
                        // -- supprime dans le dictionnaire le feature supprimer
                        //var retour = _featureImageMap.Remove(_viewPhoto);

                        // -- suppression dans liste des points qui sont chargés dans la map
                        int index = MapStateService?.CurrentLocalisations?.ToList().FindIndex(l => l.Id == LocalisationPhoto.Id) ?? -1;
                        if ( index != -1 && MapStateService is not null && MapStateService.CurrentLocalisations is not null)
                        { 
                            var localisation = new Localisation
                            {
                                Accuracy = LocalisationPhoto.Accuracy,
                                Altitude = LocalisationPhoto.Altitude,
                                Course = LocalisationPhoto.Course,
                                Id = LocalisationPhoto.Id,
                                Latitude = LocalisationPhoto.Latitude,
                                Longitude = LocalisationPhoto.Longitude,
                                LocalisationGroupId = LocalisationPhoto.LocalisationGroupId,
                                Speed = LocalisationPhoto.Speed,
                                Timestamp = LocalisationPhoto.Timestamp,
                                OrderIndex = LocalisationPhoto.OrderIndex,
                                VerticalAccuracy = LocalisationPhoto.VerticalAccuracy
                            };
                            var list = MapStateService.CurrentLocalisations.ToList();
                            list[index] = localisation;
                            MapStateService.CurrentLocalisations = list;
                        }

                        HandlerCloseCard();
                        HandleOpenRequested();

                        StateHasChanged();
                    }
                    else if( result.IsUnauthorized)
                    {   // -- logout si unauthorized
                        _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                    }
                    else
                    {
                        if (_toastService is not null)
                            await _toastService.AvertissementAsync($"Erreur lors de la suppression de la photo.");
                    }
                    
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Message error OpenMap method DeletePhoto : {ex.Message}");
                // -- affichage d"un message Toasts d'erreur de boostrap fait en javascript
                if (_toastService is not null)
                    await _toastService.ErreurAsync($"Erreur lors de la suppression de la photo.");
            }
        }
        #endregion

        #region 
        /// <summary>
        /// Initialise la Map pour charger les Pins selon la région passé en paramètre 
        /// </summary>
        /// <param name="map"></param>
        public void InitMap(Mapsui.Map map)
        {
            map.Navigator.ViewportChanged += async (s, e) =>
            {
                _cts?.Cancel();
                _cts = new CancellationTokenSource();
                try
                {
                    await Task.Delay(tpsTempo, _cts.Token);          // debounce
                    int id = MapStateService?.CurrentLocalisations?.FirstOrDefault()?.LocalisationGroupId ?? 0;
                    var b = GetGeoBounds(map, id);
                    if (b is null ) return;

                    // -- appel vers Api
                    if (_serviceHttp is not null)
                    {
                        var result = await _serviceHttp.GetPinByRegionBounds(b);
                        if (result.IsSuccess)
                        {
                            // var visibles = RemoveTooClose(pins, 40 * b.MetersPerPixel);
                            // mise à jour de la couche de pins…
                            // -- 
                            var pins = result.PinRegion.Select(p => p.ToPin()).ToList();
                            // -- chargement des points
                            var pointLayer = AddPointImage(pins);
                            if (pointLayer is not null && MapControl is not null)
                                MapControl.Map.Layers.Add(pointLayer);

                        }
                        else if (result.IsUnauthorized)
                        {   // -- logout si unauthorized
                            _navigationManager?.NavigateTo($"Account/Logout", forceLoad: true);
                        }
                    }

                }
                catch (TaskCanceledException) { }
            };
        }

        /// <summary>
        /// Retourne les limites de la régions pour retourner les pins
        /// dans l'espace de la map
        /// </summary>
        /// <param name="map"></param>
        /// <returns></returns>
        public static RegionBountsDTO? GetGeoBounds(Mapsui.Map map, int idIgnore)
        {
            var vp = map.Navigator.Viewport;
            if (vp.Width <= 0 || vp.Height <= 0) return null;   // carte pas encore dimensionnée

            var extent = vp.ToExtent();                          // MRect en mètres Mercator
            // -- sud ouest
            var sw = SphericalMercator.ToLonLat(extent.MinX, extent.MinY);
            // -- nord est 
            var ne = SphericalMercator.ToLonLat(extent.MaxX, extent.MaxY);
            // -- centre 
            // var c = SphericalMercator.ToLonLat(vp.CenterX, vp.CenterY);

            // Resolution = unités Mercator par pixel ; on corrige par cos(lat) pour obtenir des mètres réels
            // double mpp = vp.Resolution * Math.Cos(c.lat * Math.PI / 180);
            
            return new RegionBountsDTO()
            {
                IgnoredId = idIgnore,
                MinBounds = new() { Latitude = Math.Max(-90, sw.lat), Longitude = Math.Max(-180, sw.lon) },
                MaxBounds = new() { Latitude = Math.Min(90, ne.lat), Longitude = Math.Min(180, ne.lon) }
            };
        }


        #endregion

        #region private methods
        /// <summary>
        /// method qui est appelé lors de la transmission des localisations
        /// </summary>
        private void HandleOpenRequested()
        {
            if( MapStateService?.CurrentLocalisations is not null && MapStateService?.CurrentLocalisations?.Count() > 1)
            { // -- affichage des localisations
                InitMap();
                HandlerCloseCard();
                ChargingTraceAndPoint(MapStateService.CurrentLocalisations);
                StateHasChanged();
            }
            else if( MapStateService?.CurrentLocalisations is not null && MapStateService?.CurrentLocalisations?.Count() == 1)
            { // -- affichage pour un point
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

            // -- probleme avec GeometryFreature quand les points sont trop proche
            // -- zoom to box a du mal à aller sur les points indiqués 
            if ( localisations.Count() > 5 )
                ZoomToBox(outLine.Geometry?.EnvelopeInternal);
            else 
                ZoomTo(localisations.First(), 0.1);

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
        /// Affiche les points layer pour les photos
        /// </summary>
        /// <param name="localisations"></param>
        /// <returns></returns>
        private MemoryLayer? AddPointImage(IEnumerable<Pin> pins)
        { 
            if (_pinLayer is not null)
                MapControl?.Map?.Layers.Remove(_pinLayer);

            // -- vérification si des enregistrements dans la liste
            if ( !pins.Any() ) return default;
            // -- retour du feature qui retourné dans le MemoryLayer
            var features = new List<Mapsui.IFeature>();

            foreach ( var pin in pins )
            {
                if (string.IsNullOrEmpty(pin.Path)) continue;

                // -- transformation des points longitude et latitude en points mercator
                var coordonate = SphericalMercator.FromLonLat(pin.PinCoordonnee.Longitude, pin.PinCoordonnee.Latitude);
                var feature = new PointFeature(coordonate);

                feature.Styles.Add(OtherPin());
                // -- ajout dans le dictionnaire pour retrouver les photos par rapport 
                // -- à leur feature
                _featurePinMap.Add(feature, pin);

                // -- ajout dans la liste des features pour l'intégrer dans le layer
                features.Add(feature);
            }

            var memoryLayer = new MemoryLayer
            {
                Name = "Pin",
                Features = features
            };

            _pinLayer = memoryLayer;

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

            var (x, y) = SphericalMercator.FromLonLat(env.MinX, env.MinY);
            var maxPoint = SphericalMercator.FromLonLat(env.MaxX, env.MaxY);

            var box = new MRect(x, y, maxPoint.x, maxPoint.y);

            MapControl?.Map.Navigator.ZoomToBox(box, duration: 500);
            MapControl?.Refresh();
        }

        /// <summary>
        /// initialise la map en supprimant le tracé & les points photos si déjà affiché
        /// </summary>
        private void InitMap ()
        {
            if (_trace is not null)
                MapControl?.Map?.Layers.Remove(_trace);

            if (_photoLayer is not null)
                MapControl?.Map?.Layers.Remove(_photoLayer);

            if (_pinLayer is not null)
                MapControl?.Map?.Layers.Remove(_pinLayer);

            if (_clusterLayer is not null)
                MapControl?.Map?.Layers.Remove(_clusterLayer);

        }

        /// <summary>
        /// Retourne le style Pin pour qu'il soit afficher
        /// </summary>
        /// <returns></returns>
        private static ImageStyle OtherPin()
        {

            var svg = $"<svg xmlns=\"http://www.w3.org/2000/svg\" width=\"16\" height=\"16\" fill=\"currentColor\" class=\"bi bi-geo-alt\" viewBox=\"0 0 16 16\">\r\n  <path d=\"M12.166 8.94c-.524 1.062-1.234 2.12-1.96 3.07A32 32 0 0 1 8 14.58a32 32 0 0 1-2.206-2.57c-.726-.95-1.436-2.008-1.96-3.07C3.304 7.867 3 6.862 3 6a5 5 0 0 1 10 0c0 .862-.305 1.867-.834 2.94M8 16s6-5.686 6-10A6 6 0 0 0 2 6c0 4.314 6 10 6 10\"/>\r\n  <path d=\"M8 8a2 2 0 1 1 0-4 2 2 0 0 1 0 4m0 1a3 3 0 1 0 0-6 3 3 0 0 0 0 6\"/>\r\n</svg>";
            var imageStyle = new ImageStyle
            {
                Image = new Image { Source = $"svg-content://{svg}" },
                SymbolScale = 1,
            };

            return imageStyle;
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
