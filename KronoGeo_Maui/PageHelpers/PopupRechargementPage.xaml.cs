using CommunityToolkit.Maui.Views;

namespace KronoGeo_Maui.PageHelpers;

public partial class PopupRechargementPage : Popup<string>
{
	public PopupRechargementPage()
	{
		InitializeComponent();
		this.BindingContext = this;
	}
}