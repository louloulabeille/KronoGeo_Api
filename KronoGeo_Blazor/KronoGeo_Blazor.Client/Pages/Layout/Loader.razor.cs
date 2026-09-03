using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class LoaderBase : ComponentBase, IDisposable
    {
        #region public properties parameter
        [Parameter]
        public RenderFragment ChildContent { get; set; } = default!;
        #endregion

        #region protected properties view
        protected bool ShowLoaderMessage { get; set; } = false;
        // - token pour annuler le timer si le composant est détruit avant la fin du timer
        //protected CancellationTokenSource Cts { get; set; } = new();
        #endregion

        #region override protected method
        protected override Task OnInitializedAsync()
        {
            ShowLoaderMessage = true;
            return base.OnInitializedAsync();
        }
        #endregion


        #region method interface IDisposable
        public void Dispose()
        {
            //Cts.Dispose();
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
