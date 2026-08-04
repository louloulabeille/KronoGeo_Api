using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Views;
using System;
using System.Collections.Generic;
using System.Text;

namespace KronoGeo_Maui.Applications.Interface
{
    public interface IDialogService
    {
        public void ShowPopup(Popup popup);
        public Task ClosePopup(Popup popup);
        public Task<string> ShowPopupAsync(Popup popup, IPopupOptions options, CancellationToken token);
    }
}
