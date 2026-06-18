using AndroidX.Lifecycle;
using CommunityToolkit.Maui.Behaviors;
using KronoGeo_Maui.ModelViews;
//using Map = Microsoft.Maui.Controls.Maps.Map;

namespace KronoGeo_Maui;

public partial class ApplicationPage : ContentPage
{
	public ApplicationPage(ApplicationPageViewModel modelView )
	{
		InitializeComponent();
		BindingContext = modelView;

        // Ajout du behavior directement en C#
        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Appearing),
            Command = modelView.AppearingExeCommand,
            CommandParameter = this
        });
    }


}