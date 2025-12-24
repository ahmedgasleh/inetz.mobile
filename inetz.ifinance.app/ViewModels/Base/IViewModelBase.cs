using CommunityToolkit.Mvvm.Input;

namespace inetz.ifinance.app.ViewModels.Base
{
    public interface IViewModelBase
    {
        IAsyncRelayCommand InitializeAsyncCommand { get; }
    }
}
