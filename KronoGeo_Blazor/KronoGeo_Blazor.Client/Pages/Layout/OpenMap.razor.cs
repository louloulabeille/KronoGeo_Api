using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class OpenMapBase: ComponentBase
    {
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
