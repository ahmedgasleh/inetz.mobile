using inetz.ifinance.app.ViewModels;

namespace inetz.ifinance.app.Views;

public partial class RegistrationStep1Page : ContentPage
{
	public RegistrationStep1Page( RegistrationStep1ViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
    }
}