using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages
{
    public class MapBase : ComponentBase
    {
        protected MapControl? MapControl { get; set; }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (firstRender)
            {
                if ( MapControl != null )
                    MapControl.Map?.Layers.Add(Mapsui.Tiling.OpenStreetMap.CreateTileLayer());
            }
        }
    }
}
