using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.auth.app.models;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.Views;

using System.Text.Json;



namespace inetz.ifinance.app.ViewModels
{ 
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class RegistrationStep2ViewModel : ObservableObject
    {
        private readonly ApiService _api;       
        private readonly INavigationService _navigationService;

       
        public RegistrationStep2ViewModel ( ApiService api, INavigationService navigationService )
        {
           _api = api;
              _navigationService = navigationService;   
        }

        [ObservableProperty]
        private string? userId;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CompleteCommand))]
        private string lastName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CompleteCommand))]
        private string firstName = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(CompleteCommand))]
        private string address = string.Empty;

        [ObservableProperty]
        private string? errorMessage;

        [ObservableProperty]
        private bool isBusy = false;



        [RelayCommand(CanExecute = nameof(CanComplete))]
        public async Task CompleteAsync ()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                CompleteCommand.NotifyCanExecuteChanged();

                var result = await _api.PostAsync<object>("api/auth/register2", new UpdateProfile
                {
                    LastName = LastName?.Trim() ?? string.Empty,
                    FirstName = FirstName?.Trim() ?? string.Empty,
                    Address = Address?.Trim() ?? string.Empty,
                    UserId = UserId?.Trim() ?? string.Empty
                });

                if (result?.IsSuccess != true)
                {
                    ErrorMessage = result?.Error ?? "Server rejected the request.";
                    return;
                }

                await _navigationService.GoToLogin("login");
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
                CompleteCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanComplete ()
        {
            return !IsBusy
                && !string.IsNullOrWhiteSpace(FirstName)
                && !string.IsNullOrWhiteSpace(LastName)
                && !string.IsNullOrWhiteSpace(Address)
                && !string.IsNullOrWhiteSpace(UserId);
        }
    }
}
