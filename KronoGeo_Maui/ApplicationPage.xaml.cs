using AndroidX.Lifecycle;
using CommunityToolkit.Maui.Behaviors;
using CommunityToolkit.Mvvm.Messaging;
using KronoGeo_Maui.Applications.Message;
using KronoGeo_Maui.ModelViews;
using Microsoft.Maui.Controls.Maps;
using Microsoft.Maui.Maps;
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
            /*if (e.PropertyName == nameof(ApplicationPageViewModel.MapRegion) && modelView.MapRegion is not null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    googleMap.MoveToRegion(modelView.MapRegion);
                });
            }*/
            if(e.PropertyName == nameof(ApplicationPageViewModel.Location) && modelView.Location is not null )
            {
                if (googleMap.MapElements.Count == 0)
                {
                    //googleMap.MoveToRegion(MapSpan.FromCenterAndRadius(modelView.Location, Distance.FromMeters(500)));

                    var polyne = new Polyline()
                    {
                        StrokeColor = Colors.Blue,
                        StrokeWidth = 10,
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

        /*// Ajout du behavior directement en C#
        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Appearing),
            Command = modelView.AppearingExeCommand,
            CommandParameter = this
        });
        */
        // Ajout du behavior directement en C#
        /*this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Loaded),
            Command = modelView.LoadedExeCommand
        });
        */
    }

}