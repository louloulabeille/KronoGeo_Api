using CommunityToolkit.Maui;
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
            Application.Current?.Windows[0]?.Page?.ShowPopup(popup);
        }

        public async Task<string> ShowPopupAsync(Popup popup, IPopupOptions? options,CancellationToken token)
        {
            var app = Application.Current;
            if (app?.Windows?.Count > 0)
            {
                var page = app.Windows[0]?.Page;
                if (page is not null)
                {
                    var result = await page.ShowPopupAsync(popup, options, token);
                    return result?.ToString() ?? string.Empty;
                }
            }
            return string.Empty;
        }

    }
}
