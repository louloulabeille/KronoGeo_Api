using CommunityToolkit.Maui.Views;

namespace KronoGeo_Maui.PageHelpers;

public partial class PopupRechargementPage : Popup<string>
{
	public PopupRechargementPage()
	{
		InitializeComponent();
	}


    #region public method
    public async void OnValid_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync("true");
    }

    public async void OnCancel_Clicked(object sender, EventArgs e)
    {
        await this.CloseAsync("false");
    }
    #endregion

}