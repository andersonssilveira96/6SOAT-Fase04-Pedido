using Domain.Producer;
using System.Text.Json;
using System.Text;
using RabbitMQ.Client;
using System.Text.Json.Serialization;

namespace Infra.MessageBroker
{
    public class MessageBrokerProducer : IMessageBrokerProducer
    {
        public async Task SendMessageAsync<T>(T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq-service"
            };

            var connection = await factory.CreateConnectionAsync();

            using var channel = await connection.CreateChannelAsync();

            await channel.QueueDeclareAsync("pedidos-novos", exclusive: false);

            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(message, options);
            var body = Encoding.UTF8.GetBytes(json);

            await channel.BasicPublishAsync(exchange: "", routingKey: "pedidos-novos", body: body);
        }
    }
}
