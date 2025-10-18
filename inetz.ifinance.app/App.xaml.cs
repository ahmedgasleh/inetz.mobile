using inetz.ifinance.app.services;
using inetz.ifinance.app.views;

namespace inetz.ifinance.app
{
    //public partial class App : Application
    //{
    //    private readonly AuthService _authService;
    //    public App ( AuthService authService )
    //    {
    //        InitializeComponent();

    //        MainPage = new AppShell(authService);
    //    }
    //}

    public partial class App : Application
    {
        //private readonly AuthService _authService;
        public App ()
        {
            InitializeComponent();
            MainPage = new AppShell();
        }
    }

}
