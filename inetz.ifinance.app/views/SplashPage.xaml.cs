using inetz.ifinance.app.services;
using inetz.ifinance.app.viewmodels;
using System.Threading.Tasks;

namespace inetz.ifinance.app.views;

public partial class SplashPage : ContentPage
{
    SplashViewModel vm;
    public SplashPage ( SplashViewModel viewModel )
    {
        InitializeComponent();
        BindingContext = vm = viewModel;

        // Kick off the startup check (ViewModel command)
        //_ = viewModel.CheckStartupCommand.ExecuteAsync(null);

       
    }


    protected override async void OnAppearing ()
    {
        base.OnAppearing();

    // Kick off the startup check after page appears (Shell is ready)
    await vm.CheckStartupAsync();
    }
    
}