using KronoGeo_MvvM_Maui.ViewModels;

namespace KronoGeo_MvvM_Maui
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }
}
