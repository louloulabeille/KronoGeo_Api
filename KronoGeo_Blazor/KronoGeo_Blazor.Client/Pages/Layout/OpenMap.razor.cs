using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Components;
using KronoGeo_Api.Models;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class OpenMapBase: ComponentBase
    {
        #region public properties
        [Parameter]
        public List<Localisation>? Localisations { get; set; }
        #endregion

        #region protected properties
        protected MapControl? MapControl;
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
        #endregion
    }
}
