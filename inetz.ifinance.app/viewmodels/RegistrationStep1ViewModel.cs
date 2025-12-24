
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
        private string phoneNumber = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string email = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(NextCommand))]
        private string password = string.Empty;
       
        [ObservableProperty] 
        private string? errorMessage;



        [RelayCommand(CanExecute = nameof(CanNext))]
        private async void Next ()
        {
           
                Console.WriteLine("Form is busy ... !");          
           

            try
            {
                // Example: call your API to prevalidate or register
                // This assumes IApiService.RegisterStep1Async returns an ApiResult with Success + ErrorMessage
                var result = await _api.PostAsync<object>("api/auth/register1", new UserProfile
                {
                    UserId = PhoneNumber!,
                    UserEmail = Email!,
                    UserPassWord = Password!
                });

                if (!result.IsSuccess)
                {
                    // show server-side validation error (e.g., email already exists)
                    ErrorMessage = result.Error ?? "Server rejected the request.";
                    return;
                }

                var data = JsonSerializer.Deserialize<dynamic>(result?.Data.ToString());

                var id = data?.GetProperty("deviceId").ToString();

                //var data1 = JsonSerializer.Deserialize<UserProfile>(data.ToString());

                //var deid = await _deviceService.GetDeviceIdAsync();

                if (!string.IsNullOrWhiteSpace(id))
                {
                    _deviceService.CreateDeviceIdAsync(id!);
                }
                else return;

                await _navigationService.GoToRegisterUpdate(PhoneNumber);

            }
            catch (Exception ex)
            {

                throw;
            }
            finally
            {
               
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
