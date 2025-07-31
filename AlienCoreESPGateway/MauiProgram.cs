using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.EntityFrameworkCore.SqlServer;
using Microsoft.Extensions.Configuration;
using System.Reflection;
using Syncfusion.Blazor;
using pax.BlazorChartJs;
namespace AlienCoreESPGateway
{
    public static class MauiProgram
    {
        public static MauiApp CreateMauiApp()
        {
            var builder = MauiApp.CreateBuilder();

            var asm = Assembly.GetExecutingAssembly();
            using var stream = asm.GetManifestResourceStream("AlienCoreESPGateway.appsettings.json");
            if (stream == null)
                throw new InvalidOperationException("Cannot find embedded resource appsettings.json");

            var config = new ConfigurationBuilder()
                            .AddJsonStream(stream)
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
            var apiBase = builder.Configuration["ApiBaseUrl"]
              ?? "http://localhost:5000";
            builder.Services.AddScoped(sp =>
                new HttpClient { BaseAddress = new Uri(apiBase) }
            );

            builder.Services.AddSingleton(sp =>
                new HubConnectionBuilder()
                .WithUrl(hubURL)
                .WithAutomaticReconnect()
                .Build()
                );
            builder.Services.AddChartJs(options =>
            {
                options.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js";
                options.ChartJsPluginDatalabelsLocation =
                  "https://cdn.jsdelivr.net/npm/chartjs-plugin-datalabels@2";
            });

            builder.Services.AddDbContextFactory<RegisterDBContext>(options =>
                options.UseSqlServer(sqlConn)
            );
            builder.Services.AddScoped<RegisterDatabaseService>();
            builder.Services.AddSingleton<SessionState>();
            builder.Services.AddChartJs(o =>
            {
                o.ChartJsLocation = "https://cdn.jsdelivr.net/npm/chart.js@4.4.0/dist/chart.umd.min.js";
            });
            builder.Services.AddSyncfusionBlazor();
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
