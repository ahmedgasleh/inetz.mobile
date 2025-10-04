
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

        private readonly AuthService _authService;
        private readonly INavigation _navigation;

        public RegistrationStep2ViewModel ( AuthService authService, INavigation navigation )
        {
            _authService = authService;
            _navigation = navigation;
        }

        [RelayCommand]
        private async Task CompleteAsync ()
        {
            var model = new UserRegistration
            {
                PhoneNumber = Preferences.Get("TempPhone", ""),
                Email = Preferences.Get("TempEmail", ""),
                Password = Preferences.Get("TempPassword", ""),
                Name = Name,
                Address = Address,
                DeviceId = $"{DeviceInfo.Current.Platform}-{DeviceInfo.Current.Model}"
            };

            var result = await _authService.RegisterAsync(model);

            if (result?.Success == true)
            {
                await _navigation.PushAsync(new LoginPage());
            }
            else
            {
                ErrorMessage = result?.Message ?? "Registration failed.";
            }
        }
    }
}
