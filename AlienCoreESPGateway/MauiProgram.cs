using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using System.Reflection;
namespace AlienCoreESPGateway
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("AlienCoreESPGateway.appsettings.json");
            var config = new ConfigurationBuilder()
                .AddJsonStream(stream!)
                .Build();

            builder.Configuration.AddConfiguration(config);

            builder
                .UseMauiApp<App>()
                .ConfigureFonts(fonts =>
                {
                    fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                });

            var hubURL = builder.Configuration["SignalR:HubURL"]!;
            var sqlConn = builder.Configuration.GetConnectionString("DefaultConnection")!;

            builder.Services.AddSingleton(sp =>
                new HubConnectionBuilder()
                .WithUrl(hubURL)
                .WithAutomaticReconnect()
                .Build()
                );

            builder.Services.AddDbContextFactory<RegisterDBContext>(options =>
                options.UseSqlServer(sqlConn)
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
