using inetz.ifinance.app.services;

namespace inetz.ifinance.app
{
    public partial class App : Application
    {
        private readonly AuthService _authService;
        public App ( AuthService authService )
        {
            InitializeComponent();

            MainPage = new AppShell(authService);
        }
    }
}
