using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.ViewModel.Base;


namespace inetz.ifinance.app.ViewModel
{
    public partial class SplashViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        public SplashViewModel ( INavigationService navigationService )
        {
            _navigationService = navigationService;
        }

        

        [RelayCommand]
        public async Task CheckStartupAsync ()
        {
            await _navigationService.GoToRegister("//register1");

            return;
        }

    }
}
