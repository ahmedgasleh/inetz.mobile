using inetz.ifinance.app.Models;
using inetz.ifinance.app.Views.Base;
using System.Collections.ObjectModel;

namespace inetz.ifinance.app.Views;

public partial class HomePage : ContentPageBase
{
	public ObservableCollection<Account> Accounts { get; set; }
	
    public HomePage()
	{
		InitializeComponent();

		//Temporary data for testing

		InitializeAcounts();
		BindingContext = this;
    }

    //Temporary method for testing
    private void InitializeAcounts ()
    {
        Accounts = new ObservableCollection<Account>
        {
            new Account { AccountType = "Checking Account", AccountNumber = "123456789", Balance = 2500.75m },
            new Account { AccountType = "Savings Account", AccountNumber = "987654321", Balance = 10500.00m },
            new Account { AccountType = "Credit Card", AccountNumber = "5555666677778888", Balance = -1500.50m },
        };
    }
}