using CommunityToolkit.Maui;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.ViewModels;
using inetz.ifinance.app.Views;
using Microsoft.Extensions.Logging;

namespace inetz.ifinance.app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp ()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .UseMauiCommunityToolkit()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                });

#if DEBUG
    		builder.Logging.AddDebug();
#endif
            // Register Services
            builder.Services.AddSingleton<ApiService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<DeviceService>();

            // Pages (transient so a new VM/page instance is created on navigation)
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<RegistrationStep1Page>();
            builder.Services.AddTransient<RegistrationStep2Page>();
            builder.Services.AddTransient<LoginPage>();


            // Register ViewModels
            builder.Services.AddTransient<RegistrationStep1ViewModel>();
            builder.Services.AddTransient<RegistrationStep2ViewModel>();
            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<SplashViewModel>();

            return builder.Build();
        }
    }
}
