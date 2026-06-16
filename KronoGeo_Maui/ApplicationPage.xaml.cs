using KronoGeo_Maui.ModelViews;

namespace KronoGeo_Maui;

public partial class ApplicationPage : ContentPage
{
	public ApplicationPage(ApplicationPageViewModel modelView )
	{
		InitializeComponent();
		BindingContext = modelView;
	}
}