using CommunityToolkit.Maui.Behaviors;
using KronoGeo_Maui.ModelViews;

namespace KronoGeo_Maui;

public partial class ParametragePage : ContentPage
{
    public ParametragePage(ParametragePageViewModel viewModel)
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Ajout du behavior directement en C#
        this.Behaviors.Add(new EventToCommandBehavior
        {
            EventName = nameof(Appearing),
            Command = viewModel.AppearingExeCommand
        });

    }

    protected override bool OnBackButtonPressed()
    {
        Shell.Current.GoToAsync("ApplicationPage");
        return true; // -- true pour annuler le comportement par defaut
    }

}