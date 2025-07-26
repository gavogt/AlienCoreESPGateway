using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using RabbitMQ.Client;
using XenoBridge;

Host.CreateDefaultBuilder(args)
    .ConfigureServices((hostCtx, services) =>
    {
        // read RabbitMQ settings from environment 
        var rmqHost = hostCtx.Configuration["RABBITMQ_HOST"] ?? "quantumqueue";
        var rmqUser = hostCtx.Configuration["RABBITMQ_USER"] ?? "scout";
        var rmqPass = hostCtx.Configuration["RABBITMQ_PASS"] ?? "passw0rd";

        services.AddSingleton(sp => new ConnectionFactory
        {
            HostName = rmqHost,
            UserName = rmqUser,
            Password = rmqPass
        });

        services.AddHostedService<Worker>();
    })
    .Build()
    .Run();