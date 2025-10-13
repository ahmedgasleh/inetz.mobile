
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.models;
using inetz.ifinance.app.services;
using inetz.ifinance.app.views;


namespace inetz.ifinance.app.viewmodels
{
    public partial class RegistrationStep2ViewModel : ObservableObject
    {
        [ObservableProperty] private string name;
        [ObservableProperty] private string address;
        [ObservableProperty] private string errorMessage;

        private readonly AuthService _auth_service;
        private readonly DeviceService _device_service;

        public RegistrationStep2ViewModel ( AuthService authService, DeviceService deviceService )
        {
            _auth_service = authService ?? throw new ArgumentNullException(nameof(authService));
            _device_service = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
        }

        [RelayCommand]
        private async Task CompleteAsync ()
        {
            try
            {
                var model = new UserRegistration
                {
                    PhoneNumber = Preferences.Get("TempPhone", ""),
                    Email = Preferences.Get("TempEmail", ""),
                    Password = Preferences.Get("TempPassword", ""),
                    Name = Name ?? string.Empty,
                    Address = Address ?? string.Empty,
                    DeviceId = await _device_service.GetOrCreateDeviceIdAsync()
                };

                var result = await _auth_service.RegisterAsync(model);

                if (result?.Success == true)
                {
                    // Clear temp prefs
                    Preferences.Remove("TempPhone");
                    Preferences.Remove("TempEmail");
                    Preferences.Remove("TempPassword");

                    // Navigate to LoginPage
                    await MainThread.InvokeOnMainThreadAsync(() =>
                        Shell.Current.GoToAsync($"//{nameof(LoginPage)}"));
                }
                else
                {
                    ErrorMessage = result?.Message ?? "Registration failed";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
