using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.Models;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;
using inetz.ifinance.app.ViewModels.Base;
using Microsoft.Maui.Graphics;
using System.Diagnostics;

namespace inetz.ifinance.app.ViewModels
{
    public partial class BinViewModel : ViewModelBase, IQueryAttributable
    {
        private readonly ApiService _api;
        private const int BinLength = 5;
        private readonly DeviceService _deviceService;
        private readonly INavigationService _navigationService;

       // public event EventHandler? BinVerifiedSuccessfully;

        public BinViewModel ( ApiService api, DeviceService deviceService, INavigationService navigationService )
        {
            _api = api;
            _deviceService = deviceService;
            _navigationService = navigationService;
        }

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

                var result = await _api.PostAsync<VerifyBinResponse>("api/auth/verifyBin", new VerifyBin { 
                
                    DeviceId = deviceId!,
                    Bin = Bin
                });

                if ( result.IsSuccess && result.Data.IsLocked)
                {
                    await Shell.Current.GoToAsync("//login");
                    return;
                }

                if (!result.IsSuccess)
                {
                    ErrorMessage = $"Invalid BIN. Attempts left: {result.Data.RemainingAttempts}";
                    Bin = string.Empty;
                    return;
                }

                await SecureStorage.SetAsync("bin_verified_v1", "true");
               
                await Shell.Current.GoToAsync("//home");
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
