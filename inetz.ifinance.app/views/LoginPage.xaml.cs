using inetz.ifinance.app.ViewModels;
using inetz.ifinance.app.Views.Base;

namespace inetz.ifinance.app.Views;

public partial class LoginPage : ContentPageBase
{
	public LoginPage( LoginViewModel vm )
	{
		InitializeComponent();

        BindingContext = vm;
    }
}