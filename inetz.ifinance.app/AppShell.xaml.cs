using inetz.ifinance.app.services;
using inetz.ifinance.app.views;
using Microsoft.Maui.Hosting;

namespace inetz.ifinance.app
{
    public partial class AppShell : Shell
    {
        //private readonly AuthService _authService;
        private bool _startupCheckRan;
        public AppShell (  )
        {
            InitializeComponent();
            ///_authService = authService;

            Routing.RegisterRoute(nameof(RegistrationStep1Page), typeof(RegistrationStep1Page));
            Routing.RegisterRoute(nameof(RegistrationStep2Page), typeof(RegistrationStep2Page));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));
            // Set initial route to splash so the Shell shows the loader first
            GoToAsync("//splash");

        }

        //protected override void OnAppearing ()
        //{
        //    base.OnAppearing();

        //    if (_startupCheckRan) return;
        //    _startupCheckRan = true;

        //    // Fire-and-forget but safe: call the async method and let it run
        //    _ = CheckRegistrationAsync();
        //}

        //private async Task CheckRegistrationAsync ()
        //{
        //    // short delay to give UI some time to settle (optional)
        //    await Task.Delay(200);

        //    var isRegistered = await _authService.IsUserRegisteredAsync();

        //    // Use Shell.Current.GoToAsync without // for registered routes
        //    if (isRegistered)
        //    {
        //        await MainThread.InvokeOnMainThreadAsync(() =>
        //            Shell.Current.GoToAsync(nameof(LoginPage)));
        //    }
        //    else
        //    {
        //        await MainThread.InvokeOnMainThreadAsync(() =>
        //            Shell.Current.GoToAsync(nameof(RegistrationStep1Page)));
        //    }
        //}
    }

    //public partial class AppShell : Shell
    //{
    //    public AppShell ()
    //    {
    //        InitializeComponent();

    //        Routing.RegisterRoute(nameof(views.SplashPage), typeof(views.SplashPage));
    //        Routing.RegisterRoute(nameof(views.RegistrationStep1Page), typeof(views.RegistrationStep1Page));
    //        Routing.RegisterRoute(nameof(views.RegistrationStep2Page), typeof(views.RegistrationStep2Page));
    //        Routing.RegisterRoute(nameof(views.LoginPage), typeof(views.LoginPage));
    //        Routing.RegisterRoute(nameof(views.HomePage), typeof(views.HomePage));

    //        // Show Splash as initial page to let it decide navigation
    //        _ = Shell.Current.GoToAsync(nameof(views.LoginPage));
    //    }
    //}

}
