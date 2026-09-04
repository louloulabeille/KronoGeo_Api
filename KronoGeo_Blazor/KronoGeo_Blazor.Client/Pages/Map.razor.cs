using System.Linq;
using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages
{
    public class MapBase : ComponentBase
    {
        #region private inject properties
        [Inject]
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;

        #endregion

        #region protected properties view
        protected bool IsLoading { get; set; } = false;
        protected List<LocalisationGroup>? LocalisationGroup { get; set; } = [];
        #endregion

        #region protected override method
        protected async override Task OnInitializedAsync()
        {
            if (_serviceHttp is not null)
            {
                var result = await _serviceHttp.GetUserGroupLocalisationAsync("");
                LocalisationGroup = result?.GroupsDTO?
                    .Select(lg => new LocalisationGroup()
                    {
                        Id = lg.Id,
                        ApplicationUserId = lg.ApplicationUserId,
                        Date = lg.Date,
                        Name = lg.Name,
                        Localisations = lg.Localisations?.Select(l => new Localisation()
                        {
                            Accuracy = l.Accuracy,
                            Altitude = l.Altitude,
                            Course = l.Course,
                            Id = l.Id,
                            Latitude = l.Latitude,
                            Longitude = l.Longitude,
                            OrderIndex = l.OrderIndex,
                            Speed = l.Speed,
                            Timestamp = l.Timestamp,
                            VerticalAccuracy = l.VerticalAccuracy,
                            LocalisationGroupId = lg.Id
                        }).ToList()
                    }).ToList();
            }

            IsLoading = true; // -- mise en place du loader
            await base.OnInitializedAsync();
        }

        protected override Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                IsLoading = false;
                StateHasChanged();
            }
            return base.OnAfterRenderAsync(firstRender);
        }
        #endregion

    }
}
