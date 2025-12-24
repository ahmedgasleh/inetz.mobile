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



        [RelayCommand] //(CanExecute = nameof(CanNext))] when implement validation
        public async Task CompleteAsync ()
        {
            try
            {
                var result = await _api.PostAsync<object>("api/auth/register2", new UpdateProfile
                {
                    LastName = LastName!,
                    FirstName = FirstName!,
                    Address = Address!,
                    UserId = UserId!

                });

                if (!result.IsSuccess)
                {
                    // show server-side validation error (e.g., email already exists)
                    ErrorMessage = result.Error ?? "Server rejected the request.";
                    return;
                }

                var data = JsonSerializer.Deserialize<dynamic>(result.Data.ToString());

                await _navigationService.GoToLogin("login");

            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        //TDO make sure update contaims must have fields
        private bool CanNext ()
        {
            //// Do simple checks only; avoid expensive regex on every keystroke.
            //if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@")) return false;
            //if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6) return false;
            //// Phone optional here — or do a very cheap check
            //if (string.IsNullOrWhiteSpace(PhoneNumber)) return false;

            return true;
        }
    }
}
