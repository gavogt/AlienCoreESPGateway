using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace XenoBridge;

public class Worker : BackgroundService
{
    readonly ConnectionFactory _factory;
    readonly ILogger<Worker> _log;

    public Worker(ConnectionFactory factory, ILogger<Worker> log)
    {
        _factory = factory;
        _log = log;
    }

    protected override Task ExecuteAsync(CancellationToken stop)
    {
        using var conn = _factory.CreateConnection();
        using var channel = conn.CreateModel();

        channel.ExchangeDeclare("xenotelemetry", ExchangeType.Fanout, durable: true);
        var queue = channel.QueueDeclare().QueueName;
        channel.QueueBind(queue, "xenotelemetry", "");

        var consumer = new EventingBasicConsumer(channel);
        consumer.Received += (_, ea) =>
        {
            var json = Encoding.UTF8.GetString(ea.Body.ToArray());
            _log.LogInformation("Telemetry: {json}", json);
            channel.BasicAck(ea.DeliveryTag, false);
        };

        channel.BasicConsume(queue, false, consumer);
        _log.LogInformation("Listening on {q}", queue);

        return Task.Delay(-1, stop);
    }
}