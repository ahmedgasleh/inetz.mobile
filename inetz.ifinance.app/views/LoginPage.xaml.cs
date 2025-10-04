using inetz.ifinance.app.viewmodels;

namespace inetz.ifinance.app.views;

public partial class LoginPage : ContentPage
{
	public LoginPage(LoginViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
	}
}