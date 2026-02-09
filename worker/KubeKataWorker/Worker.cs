namespace KubeKataWorker;

using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Diagnostics.Metrics;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IConfiguration _configuration;
    private readonly IMessageTracker _messageTracker;
    private readonly Counter<long> _processedCounter;

    public Worker(ILogger<Worker> logger, IConfiguration configuration, IMeterFactory meterFactory, IMessageTracker messageTracker)
    {
        _logger = logger;
        _configuration = configuration;
        _messageTracker = messageTracker;
        var meter = meterFactory.Create("KubeKata.Worker");
        _processedCounter = meter.CreateCounter<long>("kubekata_worker_processed_total", "Total processed messages");
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await _messageTracker.EnsureSchemaAsync();

        var factory = new ConnectionFactory { 
            HostName = _configuration["RabbitMQ:Host"] ?? "rabbitmq",
            UserName = _configuration["RabbitMQ:User"] ?? "user",
            Password = _configuration["RabbitMQ:Password"] ?? "password"
        };
        
        using var connection = await factory.CreateConnectionAsync(stoppingToken);
        using var channel = await connection.CreateChannelAsync(cancellationToken: stoppingToken);

        await channel.QueueDeclareAsync(queue: "admin-created",
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null,
                             cancellationToken: stoppingToken);

        var consumer = new AsyncEventingBasicConsumer(channel);
        consumer.ReceivedAsync += async (model, ea) =>
        {
            var body = ea.Body.ToArray();
            var message = Encoding.UTF8.GetString(body);
            var admin = JsonSerializer.Deserialize<AdminAccountDto>(message);

            if (admin != null)
            {
                if (await _messageTracker.IsProcessedAsync(admin.Id))
                {
                    _logger.LogWarning("Message {Id} already processed. Skipping.", admin.Id);
                }
                else
                {
                    _logger.LogInformation("Processing Admin Created: {Username} ({Email})", admin.Username, admin.Email);
                    
                    // Simulate Work
                    await Task.Delay(500, stoppingToken);

                    await _messageTracker.MarkAsProcessedAsync(admin.Id, admin.Username);
                    
                    _processedCounter.Add(1, new KeyValuePair<string, object?>("status", "success"));
                }
            }

            await channel.BasicAckAsync(deliveryTag: ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
        };

        await channel.BasicConsumeAsync(queue: "admin-created",
                             autoAck: false,
                             consumer: consumer,
                             cancellationToken: stoppingToken);

        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(1000, stoppingToken);
        }
    }
}
