namespace inetz.ifinance.app
{
    public partial class App : Application
    {
        public App ()
        {
            InitializeComponent();

            MainPage = new AppShell();
        }
    }
}
