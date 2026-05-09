
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

using inetz.auth.app.models;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.ViewModels.Base;

using System.Text.Json;
using System.Text.RegularExpressions;

namespace inetz.ifinance.app.ViewModels
{
    public partial class RegistrationStep1ViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ApiService _api;
        private readonly DeviceService _deviceService;
        private readonly INavigationService _navigationService;
        public RegistrationStep1ViewModel ( ApiService api, INavigationService navigationService, DeviceService deviceService )
        {
            _api = api;
            _navigationService = navigationService;
            _deviceService = deviceService;
        }

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string? phoneNumber ;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string? email = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string? password = string.Empty;
       
        [ObservableProperty] 
        private string? errorMessage;

        [ObservableProperty]
        private bool isBusy = false;



        [RelayCommand(CanExecute = nameof(CanNext))]
        private async Task Next ()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;
                NextCommand.NotifyCanExecuteChanged();

                var result = await _api.PostAsync<object>("api/auth/register1", new UserProfile
                {
                    UserId = PhoneNumber ?? string.Empty,
                    UserEmail = Email ?? string.Empty,
                    UserPassWord = Password ?? string.Empty
                });

                if (result?.IsSuccess != true)
                {
                    ErrorMessage = result?.Error ?? "Server rejected the request.";
                    return;
                }

                var data = JsonSerializer.Deserialize<RegisterStep1Response>(
                    result.Data?.ToString() ?? string.Empty);

                if (string.IsNullOrWhiteSpace(data?.DeviceId))
                {
                    ErrorMessage = "Server did not return a device ID.";
                    return;
                }

                await _deviceService.CreateDeviceIdAsync(data.DeviceId);

                await _navigationService.GoToRegisterUpdate(PhoneNumber);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
                NextCommand.NotifyCanExecuteChanged();
            }
        }

        private bool CanNext ()
        {
            // Do simple checks only; avoid expensive regex on every keystroke.
            if (string.IsNullOrWhiteSpace(Email) || !Email.Contains("@")) return false;
            if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6) return false;
            // Phone optional here — or do a very cheap check
            if (string.IsNullOrWhiteSpace(PhoneNumber)) return false;

            return true;
        }
        public void ApplyQueryAttributes ( IDictionary<string, object> query )
        {
            if (query.Count > 0)
            {
                //eventDetail = query ["Event"] as EventModel;
            }
        }
    }
}
