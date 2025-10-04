using Microsoft.Extensions.Logging;
using Microsoft.Extensions.DependencyInjection;
using CommunityToolkit.Maui;
using inetz.ifinance.app.services;
using inetz.ifinance.app.viewmodels;

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

            // Register ViewModels
            builder.Services.AddTransient<RegistrationStep1ViewModel>();
            builder.Services.AddTransient<RegistrationStep2ViewModel>();
            builder.Services.AddTransient<LoginViewModel>();

            return builder.Build();
        }
    }
}
