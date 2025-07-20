using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
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

            builder.Services.AddDbContextFactory<RegisterDBContext>(options =>
                options.UseSqlServer(
                    @"Server=.\SQLEXPRESS;Database=AlienCoreESPGateway;Trusted_Connection=True;TrustServerCertificate=True;"
                    )
            );
            builder.Services.AddScoped<RegisterDatabaseService>();
            builder.Services.AddSingleton<SessionState>();
            builder.Services.AddMauiBlazorWebView();

#if DEBUG
            builder.Services.AddBlazorWebViewDeveloperTools();
            builder.Logging.AddDebug();
#endif

            var app = builder.Build();

            using (var scope = app.Services.CreateScope())
            {
                var factory = scope.ServiceProvider.GetRequiredService<IDbContextFactory<RegisterDBContext>>();

                using var db = factory.CreateDbContext();

                bool canConnect = db.Database.CanConnect();

                System.Diagnostics.Debug.WriteLine(canConnect ? "Database connection successful." : "Database connection failed.");

            }

            return app;

        }
    }
}
