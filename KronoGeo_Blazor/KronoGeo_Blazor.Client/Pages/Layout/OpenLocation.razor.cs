using KronoGeo_Api.Interface.Service;
using KronoGeo_Api.Models;
using KronoGeo_Blazor.Client.Infrastructure.Service;
using Microsoft.AspNetCore.Components;

namespace KronoGeo_Blazor.Client.Pages.Layout
{
    public class OpenLocationBase : ComponentBase, IDisposable
    {
        #region public properties parameter
        [Parameter]
        public LocalisationGroup? LocalisationGroup { get; set; }
        [Parameter]
        public EventCallback OnCancelEdit { get; set; }
        [Parameter]
        public EventCallback OnSave { get; set; }
        #endregion

        #region inject properties
        [Inject]
        private IServiceHttpClientAssembly? _serviceHttp { get; set; } = default;
        [Inject]
        private ToastsService? _toastsService { get; set; } = default;
        #endregion

        #region protected properties view
        /// <summary>
        /// Liste des localisations associées au groupe de localisation spécifié par LocalisationGroupId.
        /// A afficher
        /// </summary>
        protected List<Localisation> Localisations { get; set; } = [];

        protected bool IsLoading { get; set; } = false;
        #endregion

        #region protected override method
        /// <summary>
        /// OnInitializedAsync is called when the component is initialized. 
        /// It is used to perform any asynchronous operations that need to be done before the component is rendered.
        /// </summary>
        /// <returns></returns>
        protected override async Task OnInitializedAsync()
        {
            IsLoading = true;
            await LoadLocalisationsAsync();
            await base.OnInitializedAsync();
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if(firstRender)
            {
                IsLoading = false;
            }   
            base.OnAfterRender(firstRender);
        }
        #endregion

        #region protected method
        /// <summary>
        /// method qui annule l'édition du groupe de localisation et ferme le composant OpenLocation
        /// par un eventcallback OnCancelEdit
        /// </summary>
        /// <returns></returns>
        protected async Task CancelEditAsync() => await OnCancelEdit.InvokeAsync();
        
        /// <summary>
        /// method qui enregistre les modifications
        /// </summary>
        /// <returns></returns>
        protected async Task SaveLocalisationGroupAsync() => await OnSave.InvokeAsync();

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
        private async Task LoadLocalisationsAsync()
        {
            try
            {
                // Load the localisations based on the LocalisationGroupId
                // This is a placeholder for your actual data loading logic
                if (_serviceHttp is null || LocalisationGroup is null) return;

                var result = await _serviceHttp.GetLocalisationsByIdAsync(LocalisationGroup.Id);

                if (result is not null && result.IsSuccess && result?.LocalisationGroupDTO is not null 
                    && result.LocalisationGroupDTO.Localisations is not null)
                {
                    Localisations.AddRange(result.LocalisationGroupDTO.Localisations.Select(l => l.Get()).ToList());
                }
                else
                {
                    if( result?.Message is not null )
                    {
                        // -- affichage de l'erreur dans le toast
                        _toastsService?.AvertissementAsync(result.Message);
                    }else
                        throw new Exception("Erreur lors du chargement des localisations : résultat nul ou invalide.");
                }
                
            }
            catch(Exception ex)
            {
                // Handle exceptions (e.g., log the error)
                Console.WriteLine($"Error loading localisations: {ex.Message}");
                // -- affichage de l'erreur dans le toast
                _toastsService?.ErreurAsync("Erreur lors du chargement des localisations.");
            }
            
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
