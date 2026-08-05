using CommunityToolkit.Mvvm.Messaging;

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

        /*#region private properties
        private bool _isStarting = false;
        #endregion*/


        #region method private routing
        /// <summary>
        /// liste de routage dans l'application
        /// </summary>
        private static void RoutingRegister()
        {
            Routing.RegisterRoute("MainPage", typeof(MainPage));
            Routing.RegisterRoute("ApplicationPage", typeof(ApplicationPage));
            Routing.RegisterRoute("ParametragePage", typeof(ParametragePage));
        }
        #endregion

        /*protected override void OnNavigating(ShellNavigatingEventArgs args)
        {
            base.OnNavigating(args);

            // Intercepte la route de l'onglet spécifique
            if (args.Target?.Location?.OriginalString.Contains("Start") == true)
            {
                args.Cancel(); // Bloque le changement de page
                _isStarting = !_isStarting; // - changement de la glyphe entre start et pause 
                fontImageStart.Glyph = _isStarting ? "&#xe1a2" : "&#xe1c4";

                // Notifie le ViewModel
                WeakReferenceMessenger.Default.Send(new ActionStartGeo(_isStarting));
            }

            if (args.Target?.Location?.OriginalString.Contains("Stop") == true)
            {
                args.Cancel(); // Bloque le changement de page

                // Notifie le ViewModel
                WeakReferenceMessenger.Default.Send(new ActionStopGeo());
            }

            if (args.Target?.Location?.OriginalString.Contains("Camera") == true)
            {
                args.Cancel(); // Bloque le changement de page

                // Notifie le ViewModel
                WeakReferenceMessenger.Default.Send(new ActionCamera());
            }
        }*/
    }


    /*#region  classe d'appel pour faire le lien avec le model view 
    // -- start la geolocalisation
    public class ActionStartGeo(bool actionStart) { public bool ActionStart { get; set; } = actionStart; }
    // -- stop la geolocalisation
    public class ActionStopGeo { } 
    // -- ouvre la camera pour photo ou ( - video sera mis en place + tard )
    public class ActionCamera { }
    #endregion*/
}
