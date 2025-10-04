using inetz.ifinance.app.services;
using inetz.ifinance.app.views;

namespace inetz.ifinance.app
{
    public partial class AppShell : Shell
    {
        private readonly AuthService _authService;
        public AppShell ( AuthService authService )
        {
            InitializeComponent();
            _authService = authService;

            Routing.RegisterRoute(nameof(RegistrationStep1Page), typeof(RegistrationStep1Page));
            Routing.RegisterRoute(nameof(RegistrationStep2Page), typeof(RegistrationStep2Page));
            Routing.RegisterRoute(nameof(LoginPage), typeof(LoginPage));
            Routing.RegisterRoute(nameof(HomePage), typeof(HomePage));

            // Kick off startup check
            Task.Run(async () => await CheckRegistrationAsync());
        }

        private async Task CheckRegistrationAsync ()
        {
            await Task.Delay(300); // small delay for smooth UI load

            var isRegistered = await _authService.IsUserRegisteredAsync();

            if (isRegistered)
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.GoToAsync($"//{nameof(LoginPage)}"));
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(async () =>
                    await Shell.Current.GoToAsync($"//{nameof(RegistrationStep1Page)}"));
            }
        }
    }
}
