using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.services;
using Microsoft.Maui.Hosting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.viewmodels
{
    public partial class SplashViewModel : ObservableObject
    {
        private readonly AuthService _auth_service;
        private readonly DeviceService _device_service;

       

        public SplashViewModel ( AuthService authService, DeviceService deviceService )
        {
            _auth_service = authService;
            _device_service = deviceService;
        }

        [RelayCommand]
        public async Task CheckStartupAsync ()
        {
            var deviceId = await _device_service.GetOrCreateDeviceIdAsync();
            System.Diagnostics.Debug.WriteLine($"DeviceId: {deviceId}");

            var isLocallyRegistered = await _auth_service.IsUserRegisteredAsync();
            bool isRegisteredOnServer = false;

            if (isLocallyRegistered)
            {
                try
                {
                    isRegisteredOnServer = await _auth_service.IsUserRegisteredAsync();
                }
                catch
                {
                    isRegisteredOnServer = true; // fallback to local claim when offline
                }
            }

            if (!isLocallyRegistered)
            {
                //await MainThread.InvokeOnMainThreadAsync(() =>
                //    Shell.Current.GoToAsync($"//{nameof(views.RegistrationStep1Page)}"));
               
                await Shell.Current.GoToAsync("//register1");
                
                return;
            }

            if (!isRegisteredOnServer)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.GoToAsync($"//{nameof(views.RegistrationStep1Page)}"));
                return;
            }

            var loggedIn = await _auth_service.IsUserLoggedInAsync();
            if (loggedIn)
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.GoToAsync($"//{nameof(views.HomePage)}"));
            }
            else
            {
                await MainThread.InvokeOnMainThreadAsync(() =>
                    Shell.Current.GoToAsync($"//{nameof(views.LoginPage)}"));
            }
        }
    }
}
