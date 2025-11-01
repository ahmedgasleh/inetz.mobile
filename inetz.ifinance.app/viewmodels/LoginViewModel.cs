using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.ViewModels
{
    public partial class LoginViewModel : ObservableObject
    {
        [ObservableProperty] private string? phoneNumber;
        [ObservableProperty] private string? password;
        [ObservableProperty] private string? errorMessage;

        private readonly AuthService _auth_service;
        private readonly DeviceService _device_service;

        public LoginViewModel ( AuthService authService, DeviceService deviceService )
        {
            _auth_service = authService ?? throw new ArgumentNullException(nameof(authService));
            _device_service = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        }

        [RelayCommand]
        private async Task LoginAsync ()
        {
            try
            {
                var req = new LoginRequest
                {
                    PhoneNumber = PhoneNumber,
                    Password = Password,
                    DeviceId = await _device_service.GetOrCreateDeviceIdAsync()
                };

                var res = await _auth_service.LoginAsync(req);
                if (res?.Success == true)
                {
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Shell.Current.GoToAsync(nameof(HomePage)));
                }
                else
                {
                    ErrorMessage = res?.Message ?? "Login failed";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
