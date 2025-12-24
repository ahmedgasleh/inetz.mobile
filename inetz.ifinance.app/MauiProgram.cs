using CommunityToolkit.Maui;
using inetz.ifinance.app.Services;
using inetz.ifinance.app.Services.Interfaces;

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
                })
                .RegisterViewModels()
                .RegisterViews()
                .RegisterServices();

#if DEBUG
            builder.Logging.AddDebug();
#endif

           

            return builder.Build();
        }

        private static MauiAppBuilder RegisterViewModels ( this MauiAppBuilder builder )
        {
            builder.Services.AddSingleton<SplashViewModel>();
            builder.Services.AddSingleton<RegistrationStep1ViewModel>();
            builder.Services.AddSingleton<RegistrationStep2ViewModel>();
            builder.Services.AddSingleton<LoginViewModel>();
            builder.Services.AddSingleton<BinViewModel>();


            return builder;
        }

        private static MauiAppBuilder RegisterViews ( this MauiAppBuilder builder )
        {
            builder.Services.AddTransient<SplashPage>();
            builder.Services.AddTransient<RegistrationStep1Page>();
            builder.Services.AddTransient<RegistrationStep2Page>();
            builder.Services.AddTransient<LoginPage>();
            builder.Services.AddTransient<BinViewModel>();


            return builder;
        }

        private static MauiAppBuilder RegisterServices ( this MauiAppBuilder builder )
        {
            builder.Services.AddSingleton<INavigationService, NavigationService>();
            builder.Services.AddTransient<ApiService>();
            builder.Services.AddSingleton<AuthService>();
            builder.Services.AddSingleton<DeviceService>();

            return builder;
        }
    }
}
