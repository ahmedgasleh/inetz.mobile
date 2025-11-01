using inetz.ifinance.app.ViewModels;

namespace inetz.ifinance.app.Views;

public partial class LoginPage : ContentPage
{
	public LoginPage( LoginViewModel vm )
	{
		InitializeComponent();

        BindingContext = vm;
    }
}