using CommunityToolkit.Maui.Views;
using inetz.ifinance.app.ViewModels;
using inetz.ifinance.app.Views.Base;

namespace inetz.ifinance.app.Views;

public partial class Bin : ContentPageBase
{
    public Bin( BinViewModel vm )
	{
		InitializeComponent();
		BindingContext = vm;
       
    }   
    
}