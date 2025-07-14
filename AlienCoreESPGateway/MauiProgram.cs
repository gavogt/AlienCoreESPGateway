using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
namespace AlienCoreESPGateway
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
                });
            builder.Services.AddSingleton(sp => new HubConnectionBuilder()
            .WithUrl("http://192.168.1.162:5000/telemetryHub")
            .WithAutomaticReconnect()
            .Build()
            );
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            return builder.Build();
        }
    }
}
