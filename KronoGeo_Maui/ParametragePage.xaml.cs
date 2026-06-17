using KronoGeo_Maui.ModelViews;

namespace KronoGeo_Maui;

public partial class ParametragePage : ContentPage
{
	public ParametragePage( ParametragePageViewModel modelView )
	{
		InitializeComponent();
		BindingContext = modelView; 
	}
}