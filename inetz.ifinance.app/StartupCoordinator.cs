using inetz.ifinance.app.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace inetz.ifinance.app
{
    public sealed class StartupCoordinator
    {
        private readonly DeviceService _device;
        private readonly IServiceProvider _services;

        public StartupCoordinator (
            DeviceService device_service,
            IServiceProvider services )
        {
           _device = device_service;
           _services = services;
        }

        public async Task DecideAsync ()
        {
            var window = Application.Current!.Windows [0];

            var device = await _device.GetDeviceIdAsync();
            var bin = await _device.GetBinAsync();
            var binVerified = await _device.GetBinVerifyAsync();

            // ✅ Fully authenticated
            if (!string.IsNullOrWhiteSpace(bin) &&
                binVerified == "true")
            {
                window.Page = _services.GetRequiredService<AppShell>();
                return;
            }

            // 🔐 Authentication flow
            window.Page = _services.GetRequiredService<AuthShell>();

            if (string.IsNullOrWhiteSpace(device.Id))
                await Shell.Current.GoToAsync("//register1");
            else if (string.IsNullOrWhiteSpace(bin))
                await Shell.Current.GoToAsync("//login");
            else
                await Shell.Current.GoToAsync("//bin");
        }
    }

}
