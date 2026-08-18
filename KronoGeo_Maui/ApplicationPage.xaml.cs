using KronoGeo_Maui.ModelViews;
using CommunityToolkit.Maui.Behaviors;
using Microsoft.Extensions.Options;




#if ANDROID
using AndroidX.Lifecycle;
using static Android.App.Assist.AssistStructure;
#endif

//using Map = Microsoft.Maui.Controls.Maps.Map;

namespace KronoGeo_Maui;

public partial class ApplicationPage : ContentPage
{
	public ApplicationPage(ApplicationPageViewModel modelView )
	{
		InitializeComponent();
		BindingContext = modelView;
        /*modelView.PropertyChanged += (s, e) =>
        {
            // -- utilisation du message pour notifier le changement de localisation avec behavior
            *//*if (e.PropertyName == nameof(ApplicationPageViewModel.MapRegion) && modelView.MapRegion is not null)
            {
                MainThread.BeginInvokeOnMainThread(() =>
                {
                    googleMap.MoveToRegion(modelView.MapRegion);
                });
            }*//*

            // -- utilisation du message pour notifier le changement de localisation avec behavior
            *//*if(e.PropertyName == nameof(ApplicationPageViewModel.Location) && modelView.Location is not null )
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

            }*//*
        };*/

        // Ajout du behavior directement en C#
        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Appearing), // -- lors du chargement de la fenêtre
            Command = modelView.AppearingExeCommand,
        });

        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName= nameof(Disappearing), // -- sortie de la fenêtre
            Command = modelView.DisappearingExeCommand,
        });
        
        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Loaded),
            Command = modelView.LoadedExeCommand
        });

    }

}