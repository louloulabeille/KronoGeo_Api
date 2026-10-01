using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.JSInterop;


namespace KronoGeo_Api.Infrastructure.Service.Blazor
{
    public enum ToastType
    {
        Primary,
        Success,
        Danger,
        Warning,
        Info
    }


    /// <summary>
    /// Méthod pour ouvir un Toast sur le client blazor via JSInterop
    /// </summary>
    public class ToastsService
    {
        private readonly IJSRuntime _js;

        public ToastService(IJSRuntime js) => _js = js;

        /// <summary>
        /// Method générique d'ouverture de Toast sur le client blazor via JSInterop
        /// </summary>
        /// <param name="message"></param>
        /// <param name="type"></param>
        /// <param name="delaiMs"></param>
        /// <returns></returns>
        public async Task AfficherAsync(string message, ToastType type = ToastType.Primary, int delaiMs = 4000)
        {
            try
            {
                await _js.InvokeVoidAsync("afficherToastDynamique", message, type.ToString().ToLower(), delaiMs);
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
    }
}
}
