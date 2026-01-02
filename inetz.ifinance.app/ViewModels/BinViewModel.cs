using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.ViewModels.Base;
using Microsoft.Maui.Graphics;
using System.Diagnostics;
using System.Text.Json;

namespace inetz.ifinance.app.ViewModels
{
    [QueryProperty(nameof(UserId), "UserId")]
    public partial class BinViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ApiService _api;
        private const int BinLength = 5;
        private readonly DeviceService _deviceService;
        private readonly INavigationService _navigationService;
        private readonly StartupCoordinator _startupCoordinator;

        // public event EventHandler? BinVerifiedSuccessfully;

        public BinViewModel ( ApiService api, DeviceService deviceService, INavigationService navigationService, StartupCoordinator startupCoordinator )
        {
            _api = api;
            _deviceService = deviceService;
            _navigationService = navigationService;
            _startupCoordinator = startupCoordinator;
        }

        [ObservableProperty]
        private string? userId;

        [ObservableProperty]
        [NotifyCanExecuteChangedFor(nameof(AddDigitCommand))]
        private string bin = string.Empty;

        [ObservableProperty]
        private bool isBusy;

        [ObservableProperty]
        private string? errorMessage = string.Empty;

        public bool CanVerify => Bin.Length == BinLength && !IsBusy;


        public event Action<bool>? CloseRequested;

        public Color Dot1Color => Bin.Length >= 1 ? Colors.Black : Colors.LightGray;
        public Color Dot2Color => Bin.Length >= 2 ? Colors.Black : Colors.LightGray;
        public Color Dot3Color => Bin.Length >= 3 ? Colors.Black : Colors.LightGray;
        public Color Dot4Color => Bin.Length >= 4 ? Colors.Black : Colors.LightGray;
        public Color Dot5Color => Bin.Length >= 5 ? Colors.Black : Colors.LightGray;

       

        partial void OnBinChanged ( string value )
        {
            Debug.WriteLine($"Bin changed to: {value}");
            OnPropertyChanged(nameof(CanVerify));
            OnPropertyChanged(nameof(Dot1Color));
            OnPropertyChanged(nameof(Dot2Color));
            OnPropertyChanged(nameof(Dot3Color));
            OnPropertyChanged(nameof(Dot4Color));
            OnPropertyChanged(nameof(Dot5Color));
        }

        [RelayCommand(CanExecute = nameof(CanNext))]
        private void AddDigit ( string digit )
        {            
            if (Bin.Length >= BinLength || IsBusy)
                return;
            Bin += digit;
        }

        [RelayCommand]
        private void RemoveDigit ()
        {
            if (Bin.Length == 0 || IsBusy)
                return;

            Bin = Bin [..^1];
        }

        [RelayCommand]
        private async Task VerifyBinAsync ()
        {
            if (!CanVerify)
                return;

            try
            {
                IsBusy = true;
                ErrorMessage = null;

                //BinVerifiedSuccessfully?.Invoke(this, EventArgs.Empty);

                var deviceId = await SecureStorage.GetAsync("device_id_v1");
               // var result1 = await _api.VerifyBinAsync(deviceId!, Bin);

                var result = await _api.PostAsync<object>("api/auth/verifyBin", new VerifyBinRequest
                { 
                
                    UserId = UserId,
                    Bin = Bin
                });

                var data = JsonSerializer.Deserialize<dynamic>(result?.Data?.ToString());

                var isLocked = data?.GetProperty("isLocked").GetBoolean();
                var binValidated = data?.GetProperty("success").GetBoolean();

                if ( result.IsSuccess && isLocked)
                {
                    await _navigationService.GoToLogin("login");
                    return;
                }

                if (!result.IsSuccess)
                {
                    ErrorMessage = $"Invalid BIN. Attempts left: {data?.RemainingAttempts}";
                    Bin = string.Empty;
                    return;
                }

                if (binValidated)
                {
                    await SecureStorage.SetAsync("bin_verified_v1", "true");

                    //await Shell.Current.GoToAsync("//home");
                    //await _navigationService.GoToHome("home");

                    await _startupCoordinator.DecideAsync();  
                }
                else              
                    ErrorMessage = "BIN verification failed.";

            }

            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
            finally
            {
                IsBusy = false;
            }
        }

        [RelayCommand]
        private async Task GoToLoginAsync ()
        {
            await Shell.Current.GoToAsync("//login");
        }

        private bool CanNext ()
        {
            //Console.WriteLine("...!Q");
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
