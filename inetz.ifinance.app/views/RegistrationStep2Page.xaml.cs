using inetz.ifinance.app.ViewModels;
using inetz.ifinance.app.Views.Base;

namespace inetz.ifinance.app.Views;

public partial class RegistrationStep2Page : ContentPageBase
{
	public RegistrationStep2Page( RegistrationStep2ViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
    }
}