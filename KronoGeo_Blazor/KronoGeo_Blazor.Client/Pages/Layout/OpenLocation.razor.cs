using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class OpenLocationBase : ComponentBase, IDisposable
    {
        #region public properties parameter
        [Parameter]
        public int LocalisationGroupId { get; set; } = 0;
        #endregion

        #region inject properties
        [Inject]
#pragma warning disable IDE1006 // Styles d'affectation de noms
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;
#pragma warning restore IDE1006 // Styles d'affectation de noms
        #endregion

        #region protected properties view
        protected List<Localisation> Localisations { get; set; } = [];
        #endregion

        #region protected override method
        /// <summary>
        /// OnInitializedAsync is called when the component is initialized. 
        /// It is used to perform any asynchronous operations that need to be done before the component is rendered.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {

            await base.OnInitializedAsync();
        }
        #endregion

        #region pulbic method interface IDisposable
        public void Dispose()
        {
            GC.SuppressFinalize(this);
        }
        #endregion

        #region Private method

        private async Task LoadLocalisations()
        {
            // Load the localisations based on the LocalisationGroupId
            // This is a placeholder for your actual data loading logic
            //Localisations = await _serviceHttp.
        }

        #endregion
    }
}
