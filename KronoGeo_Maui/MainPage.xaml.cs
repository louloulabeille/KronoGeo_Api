using KronoGeo_Maui.ModelViews;

namespace KronoGeo_Maui
{
    public partial class MainPage : ContentPage
    {
        
        public MainPage ( MainPageViewModel binding )
        {
            InitializeComponent();
            this.BindingContext = binding;
        }
    }
}
