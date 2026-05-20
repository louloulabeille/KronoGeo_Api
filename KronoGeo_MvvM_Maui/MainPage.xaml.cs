using KronoGeo_MvvM_Maui.ViewModels;

namespace KronoGeo_MvvM_Maui
{
    public partial class MainPage : ContentPage
    {
        int count = 0;

        public MainPage()
        {
            InitializeComponent();
            BindingContext = new MainPageViewModel();
        }
    }
}
