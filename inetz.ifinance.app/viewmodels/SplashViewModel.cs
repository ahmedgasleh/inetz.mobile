using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.ViewModels.Base;


namespace inetz.ifinance.app.ViewModels
{
    public partial class SplashViewModel : ViewModelBase
    {
        private readonly INavigationService _navigationService;
        private readonly DeviceService _device_service;
        public SplashViewModel ( INavigationService navigationService, DeviceService device_service )
        {
            _navigationService = navigationService;
            _device_service = device_service;
        }

        

        [RelayCommand]
        public async Task CheckStartupAsync ()
        {
            var result = await _device_service.GetDeviceIdAsync();
            if(string.IsNullOrWhiteSpace(result.Id))
                await _navigationService.GoToRegister("//register1");
            else
                await _navigationService.GoToLogin("//login");

            return;
        }

    }
}
