using inetz.ifinance.app.ViewModel;

namespace inetz.ifinance.app.Views;

public partial class SplashPage : ContentPage
{
    SplashViewModel vm;
    public SplashPage ( SplashViewModel viewModel )
	{
		InitializeComponent ();
        BindingContext = vm = viewModel;
	}
	protected override async void OnAppearing ()
	{
		base.OnAppearing ();
		// Kick off the startup check after page appears (Shell is ready)
		await vm.CheckStartupAsync ();
    }
}