using System.Text;
using System.Text.Json;
using KubeKataApp.Application.Interfaces;
using RabbitMQ.Client;

namespace KubeKataApp.Infrastructure.Messaging;

public class RabbitMqPublisher : IMessagePublisher
{
    private readonly IConfiguration _configuration;
    private readonly string _hostname;
    private readonly string _username;
    private readonly string _password;

    public RabbitMqPublisher(IConfiguration configuration)
    {
        _configuration = configuration;
        _hostname = _configuration["RabbitMQ:Host"] ?? "rabbitmq";
        _username = _configuration["RabbitMQ:User"] ?? "user";
        _password = _configuration["RabbitMQ:Password"] ?? "password";
    }

    public async Task PublishAsync<T>(string queueName, T message)
    {
        var factory = new ConnectionFactory { 
            HostName = _hostname,
            UserName = _username,
            Password = _password
        };
        
        using var connection = await factory.CreateConnectionAsync();
        using var channel = await connection.CreateChannelAsync();

        await channel.QueueDeclareAsync(queue: queueName,
                             durable: true,
                             exclusive: false,
                             autoDelete: false,
                             arguments: null);

        var json = JsonSerializer.Serialize(message);
        var body = Encoding.UTF8.GetBytes(json);

        await channel.BasicPublishAsync(exchange: string.Empty,
                             routingKey: queueName,
                             body: body);
    }
}
