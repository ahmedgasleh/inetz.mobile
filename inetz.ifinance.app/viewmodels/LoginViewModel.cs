using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.ViewModels.Base;
using inetz.ifinance.app.Views;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace inetz.ifinance.app.ViewModels
{
    public partial class LoginViewModel : ViewModelBase, IQueryAttributable
    {    

        private readonly ApiService _api;
        private readonly DeviceService _device_service;
        private readonly INavigationService _navigationService;
        public LoginViewModel ( ApiService api, DeviceService deviceService, INavigationService navigationService )
        {
            _api = api;
            _device_service = deviceService ?? throw new ArgumentNullException(nameof(deviceService));
            _navigationService = navigationService;

        }


        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string userId = string.Empty;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(LoginCommand))]
        private string password = string.Empty;

        [ObservableProperty]
        private string? errorMessage;

        [RelayCommand(CanExecute = nameof(CanNext))]
        private async Task LoginAsync ()
        {
            try
            {
                var result = await _device_service.GetDeviceIdAsync();
               
                var res = await _api.PostAsync<object>("api/auth/login", new LoginRequest
                {
                    UserId = UserId,
                    Password = Password,
                    DeviceId = result.Id,
                });

                if (res?.IsSuccess == true)
                {
                    var data = JsonSerializer.Deserialize<dynamic>(res?.Data.ToString());

                    TimeSpan duration = DateTime.Now - DateTime.Parse(data?.GetProperty("accessTokenExpiresUtc").ToString());

                    var tokenResponse = new TokenResponse
                    {
                        AccessToken = data?.GetProperty("accessToken").ToString() ?? string.Empty,
                        RefreshToken = data?.GetProperty("refreshToken").ToString() ?? string.Empty,
                        ExpiresIn = duration.Seconds,
                    };

                    await _device_service.SaveAsync(tokenResponse);

                    var newBin = await _api.PostAsync<object>("api/auth/createBin", new LoginRequest
                    {
                        UserId = UserId,
                        Password = string.Empty,
                        DeviceId = result.Id,
                    });


                    if (newBin.IsSuccess)
                    {
                        var binData = JsonSerializer.Deserialize<dynamic>(newBin?.Data?.ToString() ?? string.Empty);
                        var bin = binData?.GetProperty("bin").ToString() ?? string.Empty;

                        var saveBin = await _api.PostAsync<object>("api/auth/saveBin", new VerifyBinRequest
                        {
                            UserId = UserId,
                            Bin = bin,
                        });


                    }

                    await MainThread.InvokeOnMainThreadAsync(() =>
                        _navigationService.GoToHome("home"));
                }
                else
                {
                    ErrorMessage = res?.Error ?? "Login failed";
                }
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }

        private bool CanNext ()
        {
            // Do simple checks only; avoid expensive regex on every keystroke.
            if (string.IsNullOrWhiteSpace(UserId)) return false;
            if (string.IsNullOrWhiteSpace(Password)) return false;
          

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
