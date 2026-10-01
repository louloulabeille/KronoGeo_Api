using Microsoft.JSInterop;

namespace KronoGeo_Blazor.Client.Infrastructure.Service
{
    public enum ToastType
    {
        Primary,
        Success,
        Danger,
        Warning,
        Info
    }

    public static class ToastAffichage
    {
        public const string TopCenter = "top-center";
        public const string TopLeft = "top-left";
        public const string TopRight = "top-right";
        public const string BottomCenter = "bottom-center";
        public const string BottomLeft = "bottom-left";
        public const string BottomRight = "bottom-right";
    }

    public class ToastsService (IJSRuntime js)
    {
        #region private readonly properties
        private readonly IJSRuntime _js = js;
        #endregion

        #region public method
        /// <summary>
        /// Method générique d'ouverture de Toast sur le client blazor via JSInterop
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="delaiMs"></param>
        /// <param name="position"></param>
        /// <returns></returns>
        public async Task AfficherAsync(string message, ToastType type = ToastType.Primary, int delaiMs = 4000, string position = ToastAffichage.TopCenter)
        {
            try
            {
                await _js.InvokeVoidAsync("afficherToastDynamique", message, type.ToString().ToLower(), delaiMs, position);
            }
            catch (JSDisconnectedException)
            {
                // Circuit Blazor Server fermé : rien à afficher
            }
        }

        public Task SuccesAsync(string message, int delaiMs = 4000) => AfficherAsync(message, ToastType.Success, delaiMs);
        public Task ErreurAsync(string message, int delaiMs = 6000) => AfficherAsync(message, ToastType.Danger, delaiMs);
        public Task AvertissementAsync(string message, int delaiMs = 5000) => AfficherAsync(message, ToastType.Warning, delaiMs);
        public Task InfoAsync(string message, int delaiMs = 4000) => AfficherAsync(message, ToastType.Info, delaiMs);
        #endregion
    }
}
