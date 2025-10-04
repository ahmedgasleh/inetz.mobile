using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.models;
using inetz.ifinance.app.services;
using inetz.ifinance.app.views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.viewmodels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string password;
        [ObservableProperty] private string errorMessage;

        private readonly AuthService _authService;
        private readonly INavigation _navigation;

        public LoginViewModel ( AuthService authService, INavigation navigation )
        {
            _authService = authService;
            _navigation = navigation;
        }

        [RelayCommand]
        private async Task LoginAsync ()
        {
            var request = new LoginRequest
            {
                PhoneNumber = PhoneNumber,
                Password = Password,
                DeviceId = $"{DeviceInfo.Current.Platform}-{DeviceInfo.Current.Model}"
            };

            var result = await _authService.LoginAsync(request);
            if (result?.Success == true)
            {
                await _navigation.PushAsync(new HomePage());
            }
            else
            {
                ErrorMessage = result?.Message ?? "Invalid login.";
            }
        }
    }
}
