
using EdgeGateway;
using MQTTnet;
using MQTTnet.Client;
using MQTTnet.Client.Options;
using MQTTnet.Client.Subscribing;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSignalR();

builder.Services.AddHostedService<TelemetryForwarder>();
builder.Services.AddSingleton<IMqttClient>(sp =>
{
    var factory = new MqttFactory();
    var mqttClient = factory.CreateMqttClient();
    var options = new MqttClientOptionsBuilder()
        .WithClientId("EdgeGateway")
        .WithTcpServer("192.168.1.164", 1883)
        .WithCleanSession()
        .Build();
    mqttClient.ConnectAsync(options, CancellationToken.None).Wait();
    return mqttClient;
});
var app = builder.Build();
app.Urls.Clear();
app.Urls.Add("http://192.168.1.162:5000");
app.MapHub<TelemetryHub>("/telemetryHub");

app.Run();
