using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Views;
using System.Text.RegularExpressions;

namespace inetz.ifinance.app.ViewModels
{
    public partial class RegistrationStep1ViewModel : ObservableObject
    {
        private readonly ApiService _api; // inject your API service (optional)
        public RegistrationStep1ViewModel ( ApiService api )
        {
            _api = api;
        }
        [ObservableProperty] private string? phoneNumber;
        [ObservableProperty] private string? email;
        [ObservableProperty] private string? password;
        [ObservableProperty] private string? errorMessage;
        [ObservableProperty] private bool isBusy;


        // When inputs change, re-evaluate CanExecute (generated NextCommand)
        partial void OnPhoneNumberChanged ( string? value ) => NextCommand.NotifyCanExecuteChanged();
        partial void OnEmailChanged ( string? value ) => NextCommand.NotifyCanExecuteChanged();
        partial void OnPasswordChanged ( string? value ) => NextCommand.NotifyCanExecuteChanged();
        partial void OnIsBusyChanged ( bool value ) => NextCommand.NotifyCanExecuteChanged();

        // NextCommand (generated) will call CanNext to decide whether the button is enabled
        [RelayCommand(CanExecute = nameof(CanNext))]
        public async Task NextAsync ()
        {
            // prevent re-entry -- IsBusy already used in CanNext but double-protect
            if (IsBusy) return;

            ErrorMessage = null; // clear previous error
            IsBusy = true;

            try
            {
                // local validation (should be true when CanNext allowed the command, but extra check)
                if (!ValidateInputs())
                {
                    ErrorMessage = "Fix input errors.";
                    return;
                }

                Preferences.Set("TempPhone", PhoneNumber);
                Preferences.Set("TempEmail", Email);
                Preferences.Set("TempPassword", Password);

                // var dd  = _api.PostAsync<object>("register", new { phoneNumber = PhoneNumber, email = Email });

                // Example: call your API to prevalidate or register
                // This assumes IApiService.RegisterStep1Async returns an ApiResult with Success + ErrorMessage
                var  result = await _api.PostAsync<object>("register", new UserRegistration
                {
                    PhoneNumber = PhoneNumber!,
                    Email = Email!,
                    Password = Password!
                });

                /*if (!result.Success)
                {
                    // show server-side validation error (e.g., email already exists)
                    ErrorMessage = result.ErrorMessage ?? "Server rejected the request.";
                    return;
                }*/

                await Shell.Current.GoToAsync(nameof(RegistrationStep2Page));
            }
            catch (OperationCanceledException)
            {
                // user cancelled - optional
            }
            catch (Exception ex)
            {

                // network or unexpected error
                ErrorMessage = "Network error — please try again.";
                System.Diagnostics.Debug.WriteLine(ex);
            }
            finally
            {
                IsBusy = false;
            }
             
        }

        private bool CanNext ()
        {
            // button enabled when not busy and local validation passes
            return !IsBusy && ValidateInputs();
        }

        public bool ValidateInputs ()
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
