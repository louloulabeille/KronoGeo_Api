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
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;
        #endregion

        #region protected properties view
        /// <summary>
        /// Liste des localisations associées au groupe de localisation spécifié par LocalisationGroupId.
        /// A afficher
        /// </summary>
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

        /// <summary>
        /// Charge les localisations basées sur l'ID du groupe de localisation.
        /// </summary>
        /// <returns></returns>
        private async Task LoadLocalisations()
        {
            // Load the localisations based on the LocalisationGroupId
            // This is a placeholder for your actual data loading logic
            if ( _serviceHttp == null ) return;
            var result = await _serviceHttp.GetLocalisationsByIdAsync(LocalisationGroupId)??null;

            if( result is null || result?.LocalisationGroupDTO is null || result.LocalisationGroupDTO.Localisations is null)
                return;
            Localisations.AddRange(result.LocalisationGroupDTO.Localisations.Select( l => l.Get()).ToList());
        }

        /// <summary>
        /// Initializes the localisations list by clearing any existing data.
        /// </summary>
        private void InitializeLocalisations()
        {
            Localisations.Clear();
        }

        #endregion
    }
}
