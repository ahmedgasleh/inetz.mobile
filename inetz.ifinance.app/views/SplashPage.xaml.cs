using inetz.ifinance.app.viewmodels;

namespace inetz.ifinance.app.views;

public partial class SplashPage : ContentPage
{
    public SplashPage ( SplashViewModel viewModel )
    {
        InitializeComponent();
        BindingContext = viewModel;

        // Kick off the startup check (ViewModel command)
       // _ = viewModel.CheckStartupCommand.ExecuteAsync(null);
    }
}