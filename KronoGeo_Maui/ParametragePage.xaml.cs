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

    /// <summary>
    /// method de gestion du bouton retour pour revenir à la page d'application
    /// impossible de créer un behavior pour le bouton retour 
    /// car il n'existe pas d'event pour ce bouton
    /// </summary>
    /// <returns></returns>
    protected override bool OnBackButtonPressed()
    {
        if (BindingContext is ParametragePageViewModel)
        {
            ParametragePageViewModel.BackButtonPressed();
            //Shell.Current.GoToAsync("ApplicationPage");
            return true; // -- true pour annuler le comportement par defaut
        }
        return base.OnBackButtonPressed();
    }

}