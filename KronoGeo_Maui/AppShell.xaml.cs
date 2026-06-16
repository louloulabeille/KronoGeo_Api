namespace KronoGeo_Maui
{
    public partial class AppShell : Shell
    {
        public AppShell()
        {
            InitializeComponent();

            // - liste de routage
            RoutingRegister();
        }


        /// <summary>
        /// liste de routage dans l'application
        /// </summary>
        private static void RoutingRegister()
        {
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("ApplicationPage", typeof(ApplicationPage));
        }
    }
}
