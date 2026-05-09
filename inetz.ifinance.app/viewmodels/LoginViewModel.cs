
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

        [ObservableProperty]
        private bool isBusy = false;

        //[RelayCommand(CanExecute = nameof(CanNext))]
        //private async Task LoginAsync ()
        //{
        //    if (IsBusy)
        //        return;

        //    try
        //    {
        //        IsBusy = true;

        //        ErrorMessage = string.Empty;

        //        var result = await _device_service.GetDeviceIdAsync();

        //        var res = await _api.PostAsync<object>("api/auth/login", new LoginRequest
        //        {
        //            UserId = UserId,
        //            Password = Password,
        //            DeviceId = result.Id,
        //        });



        //        if (res?.IsSuccess != true)
        //        {
        //            ErrorMessage = res?.Error ?? "Login failed";
        //            return;
        //        }

        //        var loginData = JsonSerializer.Deserialize<LoginResponse>(res.Data?.ToString() ?? string.Empty);

        //        if (loginData == null)
        //        {
        //            ErrorMessage = "Invalid login response";
        //            return;
        //        }

        //        var expiresIn = (int)(loginData.AccessTokenExpiresUtc - DateTime.UtcNow).TotalSeconds;

        //        if (expiresIn <= 0)
        //        {
        //            ErrorMessage = "Login token has already expired";
        //            return;
        //        }



        //        if (res?.IsSuccess == true)
        //        {
        //            var data = JsonSerializer.Deserialize<LoginResponse>(res?.Data?.ToString() ?? string.Empty);                  

        //            var expiresUtc = data?.AccessTokenExpiresUtc ?? DateTime.UtcNow;
        //            var expiresIn = (int)(expiresUtc - DateTime.UtcNow).TotalSeconds;

        //            var tokenResponse = new TokenResponse
        //            {
        //                AccessToken = data?.AccessToken ?? string.Empty,
        //                RefreshToken = data?.RefreshToken ?? string.Empty,
        //                ExpiresIn = expiresIn,
        //            };

        //            await _device_service.SaveAsync(tokenResponse);

        //            var newBin = await _api.PostAsync<object>("api/auth/createBin", new LoginRequest
        //            {
        //                UserId = UserId,
        //                Password = string.Empty,
        //                DeviceId = result.Id,
        //            });


        //            if (newBin?.IsSuccess == true)
        //            {
        //                var binData = JsonSerializer.Deserialize<BinResponse>(newBin?.Data?.ToString() ?? string.Empty);
        //                var bin = binData?.Bin ?? string.Empty;

        //                var saveBin = await _api.PostAsync<object>("api/auth/saveBin", new VerifyBinRequest
        //                {
        //                    UserId = UserId,
        //                    Bin = bin,
        //                });

        //                if (saveBin.IsSuccess)
        //                {
        //                    var sendResult = await _api.PostAsync<object>("api/auth/sendBin", new VerifyBinRequest
        //                    {
        //                        UserId = UserId,
        //                        Bin = bin,
        //                    });

        //                    if(sendResult.IsSuccess)
        //                    {
        //                        await SecureStorage.SetAsync("bin_v1", bin);
        //                        await MainThread.InvokeOnMainThreadAsync(() => _navigationService.GoToBin(UserId));
        //                    }
        //                    else
        //                    {
        //                        ErrorMessage = saveBin?.Error ?? "Bin process failed";
        //                        return;
        //                    }
        //                }
        //                else
        //                {
        //                    ErrorMessage = saveBin?.Error ?? "Saving bin failed";
        //                    return;
        //                }



        //            }
        //            else
        //            {
        //                ErrorMessage = newBin?.Error ?? "New bin failed, login agian";
        //                return;
        //            }



        //        }
        //        else
        //        {
        //            ErrorMessage = res?.Error ?? "Login failed";
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = ex.Message;
        //    }
        //}

        [RelayCommand(CanExecute = nameof(CanNext))]
        private async Task LoginAsync ()
        {
            if (IsBusy)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = string.Empty;

                var device = await _device_service.GetDeviceIdAsync();

                var loginRes = await _api.PostAsync<object>("api/auth/login", new LoginRequest
                {
                    UserId = UserId,
                    Password = Password,
                    DeviceId = device.Id,
                });

                if (loginRes?.IsSuccess != true)
                {
                    ErrorMessage = loginRes?.Error ?? "Login failed";
                    return;
                }

                var loginData = JsonSerializer.Deserialize<LoginResponse>(
                    loginRes.Data?.ToString() ?? string.Empty);

                if (loginData == null)
                {
                    ErrorMessage = "Invalid login response";
                    return;
                }

                var expiresIn = (int)(loginData.AccessTokenExpiresUtc - DateTime.UtcNow).TotalSeconds;

                if (expiresIn <= 0)
                {
                    ErrorMessage = "Login token has already expired";
                    return;
                }

                var newBin = await _api.PostAsync<object>("api/auth/createBin", new CreateBinRequest
                {
                    UserId = UserId,
                    DeviceId = device.Id,
                });

                if (newBin?.IsSuccess != true)
                {
                    ErrorMessage = newBin?.Error ?? "New BIN failed. Please login again.";
                    return;
                }

                var binData = JsonSerializer.Deserialize<BinResponse>(
                    newBin.Data?.ToString() ?? string.Empty);

                var bin = binData?.Bin;

                if (string.IsNullOrWhiteSpace(bin))
                {
                    ErrorMessage = "Invalid BIN response";
                    return;
                }

                var saveBin = await _api.PostAsync<object>("api/auth/saveBin", new VerifyBinRequest
                {
                    UserId = UserId,
                    Bin = bin,
                });

                if (saveBin?.IsSuccess != true)
                {
                    ErrorMessage = saveBin?.Error ?? "Saving BIN failed";
                    return;
                }

                var sendResult = await _api.PostAsync<object>("api/auth/sendBin", new VerifyBinRequest
                {
                    UserId = UserId,
                    Bin = bin,
                });

                if (sendResult?.IsSuccess != true)
                {
                    ErrorMessage = sendResult?.Error ?? "Sending BIN failed";
                    return;
                }

                await _device_service.SaveAsync(new TokenResponse
                {
                    AccessToken = loginData.AccessToken,
                    RefreshToken = loginData.RefreshToken,
                    ExpiresIn = expiresIn,
                });

                await SecureStorage.SetAsync("bin_v1", bin);

                await MainThread.InvokeOnMainThreadAsync(() => _navigationService.GoToBin(UserId));

               
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
                LoginCommand.NotifyCanExecuteChanged();
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
