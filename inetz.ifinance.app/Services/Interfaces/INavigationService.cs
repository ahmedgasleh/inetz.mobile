namespace inetz.ifinance.app.Services.Interfaces
{
    public interface INavigationService
    {
        Task GoBack ();

        Task GoTo ( string route );

        Task GoToSplash ( string? returnUrl );

        Task GoToRegister ( string? returnUrl );
        Task GoToRegisterUpdate ( string? userId );
        Task GoToLogin ( string? returnUrl );
        Task GoToHome ( string? returnUrl );

        //Task GoToSelectedOrderDetail ( long selectedOrderId );

        //Task GoToSelectedPieDetail ( int selectedPieId );
    }
}
