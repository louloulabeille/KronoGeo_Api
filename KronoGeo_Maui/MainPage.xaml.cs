using CommunityToolkit.Maui.Behaviors;
using KronoGeo_Maui.ModelViews;

namespace KronoGeo_Maui
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage ( MainPageViewModel viewModel )
        {
            InitializeComponent();
            this.BindingContext = viewModel;

            // Ajout du behavior directement en C#
            this.Behaviors.Add(new EventToCommandBehavior
            {
                EventName = nameof(Appearing),
                Command = viewModel.AppearingExeCommand
            });

        }
    }
}
