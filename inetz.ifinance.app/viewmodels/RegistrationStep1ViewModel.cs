using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using inetz.ifinance.app.models;
using inetz.ifinance.app.views;

namespace inetz.ifinance.app.viewmodels
{
    public partial class RegistrationStep1ViewModel : ObservableObject
    {
        [ObservableProperty] private string phoneNumber;
        [ObservableProperty] private string email;
        [ObservableProperty] private string password;
        [ObservableProperty] private string errorMessage;

        [RelayCommand]
        private async Task NextAsync ()
        {
            if (!ValidateInputs())
            {
                ErrorMessage = "Fix input errors";
                return;
            }

            Preferences.Set("TempPhone", PhoneNumber);
            Preferences.Set("TempEmail", Email);
            Preferences.Set("TempPassword", Password);

            await Shell.Current.GoToAsync(nameof(RegistrationStep2Page));
        }

        private bool ValidateInputs ()
        {
            if (string.IsNullOrWhiteSpace(PhoneNumber) || !Regex.IsMatch(PhoneNumber, @"^\+[1-9]\d{6,14}$"))
                return false;
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@"))
                return false;
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
                return false;
            return true;
        }
    }
}
