using inetz.ifinance.app.ViewModel.Base;


namespace inetz.ifinance.app.Views.Base
{
    public class ContentPageBase : ContentPage
    {
        protected override async void OnAppearing ()
        {
            base.OnAppearing();

            if (BindingContext is not IViewModelBase ivmb)
            {
                return;
            }

            await ivmb.InitializeAsyncCommand.ExecuteAsync(null);
        }
    }
}
