using CommunityToolkit.Mvvm.Input;

namespace inetz.ifinance.app.ViewModel.Base
{
    public interface IViewModelBase
    {
        IAsyncRelayCommand InitializeAsyncCommand { get; }
    }
}
