
using EdgeGateway;
using MQTTnet.Client;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddSingleton<IMqttClient>();
builder.Services.AddHostedService<TelemetryForwarder>();

var app = builder.Build();

app.MapHub<TelemetryHub>("/telemetryHub");

app.Run();
