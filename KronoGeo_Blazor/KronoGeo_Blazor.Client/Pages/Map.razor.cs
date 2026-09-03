using KronoGeo_Api.Interface.Service;
using Mapsui.UI.Blazor;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages
{
    public class MapBase : ComponentBase
    {
        #region protected properties view
        protected bool IsLoading { get; set; } = false;
        #endregion

        #region protected override method
        protected override void OnInitialized()
        {
            IsLoading = true; // -- mise en place du loader
            base.OnInitialized();
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
