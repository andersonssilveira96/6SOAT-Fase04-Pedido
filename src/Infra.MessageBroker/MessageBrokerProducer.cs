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
            // Definição do servidor Rabbit MQ : usamos uma imagem Docker
            var factory = new ConnectionFactory
            {
                HostName = "localhost"
            };

            // Cria uma conexão RabbitMQ usando uma factory
            var connection = await factory.CreateConnectionAsync();

            // Cria um channel com sessão e model
            using var channel = await connection.CreateChannelAsync();

            // declara a fila(queue) a seguir o nome e propriedades
            await channel.QueueDeclareAsync("novos-pedidos", exclusive: false);

            // Serializa a mensagem+
            JsonSerializerOptions options = new()
            {
                ReferenceHandler = ReferenceHandler.IgnoreCycles,
                WriteIndented = true
            };

            string json = JsonSerializer.Serialize(message, options);
            var body = Encoding.UTF8.GetBytes(json);

            // Põe os dados na fila : product
            await channel.BasicPublishAsync(exchange: "", routingKey: "novos-pedidos", body: body);
        }
    }
}
