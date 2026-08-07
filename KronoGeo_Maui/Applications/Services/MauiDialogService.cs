using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Core;
using CommunityToolkit.Maui.Extensions;
using CommunityToolkit.Maui.Views;
using KronoGeo_Maui.Applications.Interface;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Services
{
    internal class MauiDialogService : IDialogService
    {
        public async Task ClosePopup(Popup popup)
        {
            await popup.CloseAsync();
        }

        public void ShowPopup(Popup popup)
        {
            //Application.Current?.Windows[0]?.Page?.ShowPopup(popup);
            Shell.Current.CurrentPage.ShowPopup(popup);
        }

        public async Task<T?> ShowPopupAsync<T>(Popup popup, IPopupOptions? options,CancellationToken token) where T : class
        {
            IPopupResult<T?> result = await Shell.Current.CurrentPage.ShowPopupAsync<T>(popup, options, token);
            return result.Result;
            /*
            var app = Application.Current;
            if (app?.Windows?.Count > 0)
            {
                var page = app.Windows[0]?.Page;
                if (page is not null)
                {
                    IPopupResult<T> result = await page.ShowPopupAsync<T>(popup, options, token);
                    return result.Result;
                }
            }
            return null;*/
        }

    }
}
