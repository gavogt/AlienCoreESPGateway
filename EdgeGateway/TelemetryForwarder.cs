using System.Text;
using MQTTnet.Client;
using Microsoft.AspNetCore.SignalR;
using MQTTnet.Client.Subscribing;

namespace EdgeGateway
{
    public class TelemetryForwarder : BackgroundService
    {
        private readonly IMqttClient _mqtt;
        private readonly IHubContext<TelemetryHub> _hub;

        public TelemetryForwarder(IMqttClient mqtt, IHubContext<TelemetryHub> hub)
        {
            _mqtt = mqtt;
            _hub = hub;
        }

        protected override async Task ExecuteAsync(CancellationToken ct)
        {
            _mqtt.UseApplicationMessageReceivedHandler(async c =>
            {
                var topic = c.ApplicationMessage.Topic;
                var payload = Encoding.UTF8.GetString(c.ApplicationMessage.Payload);
                await _hub.Clients.All.SendAsync("Receive", topic, payload, ct);
            });

            var subscribeOptions = new MqttClientSubscribeOptionsBuilder()
                .WithTopicFilter("xeno/+/telemetry")
                .Build();

            await _mqtt.SubscribeAsync(subscribeOptions, ct);

            await Task.Delay(Timeout.Infinite, ct);
        }
    }
}
