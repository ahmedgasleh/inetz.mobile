using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.ViewModel.Base
{
    public partial class ViewModelBase : ObservableValidator, IViewModelBase
    {
        [ObservableProperty]
        private bool _isLoading;

        public IAsyncRelayCommand InitializeAsyncCommand { get; }

        public ViewModelBase ()
        {
            InitializeAsyncCommand = new AsyncRelayCommand(
                async () =>
                {
                    IsLoading = true;
                    await Loading(LoadAsync);
                    IsLoading = false;
                });
        }

        protected async Task Loading ( Func<Task> unitOfWork )
        {
            await unitOfWork();
        }

        public virtual Task LoadAsync ()
        {
            return Task.CompletedTask;
        }
    }
}
