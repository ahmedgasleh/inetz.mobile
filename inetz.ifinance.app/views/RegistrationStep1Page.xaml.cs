using inetz.ifinance.app.ViewModel;
using inetz.ifinance.app.Views.Base;

namespace inetz.ifinance.app.Views;

public partial class RegistrationStep1Page : ContentPageBase
{
	public RegistrationStep1Page( RegistrationStep1ViewModel vm)
	{
		InitializeComponent();
		BindingContext = vm;
    }
}