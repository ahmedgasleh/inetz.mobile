using inetz.ifinance.app.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app.Services
{
    public class NavigationService : INavigationService
    {
        public Task GoBack ()
            => Navigate("..");

        public Task GoTo ( string route )
            => Navigate(route);

        public Task GoToSplash ( string? returnUrl )
            => returnUrl is null
                ? Navigate("//splash")
                : Navigate("//splash", new Dictionary<string, object> { { "ReturnUrl", returnUrl } });

        public Task GoToRegister ( string? returnUrl )
            => returnUrl is null
                    ? Navigate("//register1")
                    : Navigate("//register1", new Dictionary<string, object> { { "ReturnUrl", returnUrl } });

        public Task GoToRegisterUpdate ( string? userId)
          =>  Navigate("//register2", new Dictionary<string, object> { ["UserId"] = userId ?? string.Empty });



        //public Task GoToSelectedOrderDetail ( long selectedOrderId )
        //    => Navigate("OrderDetail", new Dictionary<string, object> { { "OrderId", selectedOrderId } });

        //public Task GoToSelectedPieDetail ( int selectedPieId )
        //    => Navigate("PieDetail", new Dictionary<string, object> { { "PieId", selectedPieId } });

        private Task Navigate ( string route, IDictionary<string, object>? parameters = null )
        {
            var shellNavigation = new ShellNavigationState(route);

            return parameters != null
                ? Shell.Current.GoToAsync(shellNavigation, parameters)
                : Shell.Current.GoToAsync(shellNavigation);
        }
    }
}
