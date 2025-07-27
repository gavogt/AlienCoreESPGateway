
using EdgeGateway;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;

var builder = WebApplication.CreateBuilder(args);

var config = builder.Configuration;

var urlConfig = config["Urls"]
    ?? throw new InvalidOperationException("Missing `Urls` configuration");
var urls = urlConfig
    .Split(';', StringSplitOptions.RemoveEmptyEntries);
builder.WebHost.UseUrls(urls);

var mqttSection = config.GetSection("MqttOptions");
var mqttOpts = mqttSection.Get<MqttOptions>()!;

builder.Services.AddSignalR();

builder.Services.AddHostedService<TelemetryForwarder>();
builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var factory = new MqttFactory();
    var mqttClient = factory.CreateMqttClient();
    var options = new MqttClientOptionsBuilder()
        .WithClientId(mqttOpts.ClientId)
        .WithTcpServer(mqttOpts.Server, mqttOpts.Port)
        .WithCredentials("scout", "passw0rd")
        .WithCleanSession()
        .Build();

    mqttClient.ConnectAsync(options, CancellationToken.None).Wait();
    return mqttClient;
});

var app = builder.Build();

var hubPath = config["SignalR:HubPath"] ?? "/telemetryHub";

app.MapGet("/", () => Results.Redirect(hubPath));

app.MapHub<TelemetryHub>(hubPath);

app.Run();
