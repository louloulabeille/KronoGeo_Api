using AndroidX.Lifecycle;
using CommunityToolkit.Maui.Behaviors;
using KronoGeo_Maui.ModelViews;
using Microsoft.Maui.Controls.Maps;
using static Android.App.Assist.AssistStructure;
//using Map = Microsoft.Maui.Controls.Maps.Map;

namespace KronoGeo_Maui;

public partial class ApplicationPage : ContentPage
{
	public ApplicationPage(ApplicationPageViewModel modelView )
	{
		InitializeComponent();
		BindingContext = modelView;

        modelView.PropertyChanged += (s, e) =>
        {
            if (e.PropertyName == nameof(ApplicationPageViewModel.MapRegion) && modelView.MapRegion is not null)
            {
                googleMap.MoveToRegion(modelView.MapRegion);
            }
            if(e.PropertyName == nameof(ApplicationPageViewModel.Location) && modelView.Location is not null )
            {
                if (googleMap.MapElements.Count == 0)
                {
                    var polyne = new Polyline()
                    {
                        StrokeColor = Colors.Blue,
                        StrokeWidth = 12,
                    };
                    polyne.Geopath.Add(modelView.Location);
                    googleMap.MapElements.Add(polyne);
                }
                else
                {
                    var element = googleMap.MapElements.FirstOrDefault() as Polyline;
                    element?.Geopath.Add(modelView.Location);
                }

            }
        };

        // Ajout du behavior directement en C#
        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Appearing),
            Command = modelView.AppearingExeCommand,
            CommandParameter = this
        });
    }
}