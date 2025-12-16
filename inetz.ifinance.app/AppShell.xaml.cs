using inetz.ifinance.app.Views;

namespace inetz.ifinance.app
{
    public partial class AppShell : Shell
    {
        public AppShell ()
        {
            InitializeComponent();

            Routing.RegisterRoute(nameof(RegistrationStep1Page), typeof(RegistrationStep1Page));
            Routing.RegisterRoute(nameof(RegistrationStep2Page), typeof(RegistrationStep2Page));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            // Set initial route to splash so the Shell shows the loader first
            //Uncomment for full flow
            GoToAsync("//splash");
            //testing purposes
            //GoToAsync("//register1");
        }
    }
}
