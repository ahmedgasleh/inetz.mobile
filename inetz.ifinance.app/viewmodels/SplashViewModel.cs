using CommunityToolkit.Mvvm.Input;
using inetz.ifinance.app.ViewModels.Base;

namespace inetz.ifinance.app.ViewModels
{
    public partial class SplashViewModel : ViewModelBase
    {
        private readonly StartupCoordinator _coordinator;
        public SplashViewModel ( StartupCoordinator coordinator )
        {
            _coordinator = coordinator;
        }        

        [RelayCommand]
        public async Task CheckStartupAsync ()
        {
            await MainThread.InvokeOnMainThreadAsync(
                () => _coordinator.DecideAsync());
        }
    }
}
