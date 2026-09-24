using CommunityToolkit.Maui.Views;

namespace KronoGeo_Maui.PageHelpers;

public partial class PopupBatterySaverPage : Popup<string>
{
	public PopupBatterySaverPage()
	{
		InitializeComponent();
	}

    #region method public clicked

    #endregion

    private async void OpenParameter_Clicked(object? sender, EventArgs e)
    {
        await this.CloseAsync("true");
    }

    private async void Close_Clicked(object? sender, EventArgs e)
    {
        await this.CloseAsync("false");
    }
}