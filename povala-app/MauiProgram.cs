using Microsoft.Extensions.Logging;
using povala_app.Services;
using povala_app.ViewModels;
using povala_app.Views;
using LoginViewModel = povala_app.ViewModels.Auth.LoginViewModel;
using RegisterViewModel = povala_app.ViewModels.Auth.RegisterViewModel;


namespace povala_app
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();
            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                    fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
                })
                .Services.AddHttpClient("PovalaApi", client =>
                {
                    string apiIp = DeviceInfo.Platform == DevicePlatform.Android 
                        ? (DeviceInfo.DeviceType == DeviceType.Physical ? "192.168.0.102" : "10.0.2.2") 
                        : "localhost";

                    client.BaseAddress = new Uri($"http://{apiIp}:5058");
                });

            builder.Services.AddSingleton<AuthApiService>();
            builder.Services.AddSingleton<BoulderApiService>();
            builder.Services.AddSingleton<AscentsApiService>();
            builder.Services.AddSingleton<ProjectsApiService>();

            builder.Services.AddTransient<RegisterViewModel>();
            builder.Services.AddTransient<RegisterPage>();

            builder.Services.AddTransient<LoginViewModel>();
            builder.Services.AddTransient<LoginPage>();

            builder.Services.AddTransient<MapViewModel>();
            builder.Services.AddTransient<MapPage>();
            builder.Services.AddTransient<ProfilePage>();



#if DEBUG
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
