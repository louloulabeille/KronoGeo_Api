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
    }
}
