using Application.DTOs.Pedido;
using Application.UseCase.Pedidos;
using Domain.Consumer;
using Domain.Enums;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Text;
using System.Text.Json;

namespace Infra.MessageBroker
{
    public class MessageBrokerConsumer : IMessageBrokerConsumer
    {
        private readonly IPedidoUseCase _pedidoUseCase;
        private IConnection _connection;
        private IChannel _channel;

        public MessageBrokerConsumer(IPedidoUseCase pedidoUseCase)
        {
            _pedidoUseCase = pedidoUseCase;
        }

        public async Task ReceiveMessageAsync()
        {
            var factory = new ConnectionFactory
            {
                HostName = "rabbitmq-service"
            };

            _connection = await factory.CreateConnectionAsync();
            _channel = await _connection.CreateChannelAsync();

            await _channel.QueueDeclareAsync("pedidos-atualizados", exclusive: false);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, eventArgs) => {
                var body = eventArgs.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                Console.WriteLine($"Order message received: {message}");

                var pedido = JsonSerializer.Deserialize<PedidoDto>(message)!;

                Console.WriteLine($"Pedido: { JsonSerializer.Serialize(message) }");

                await _pedidoUseCase.AtualizarStatus(pedido.Id, !string.IsNullOrEmpty(pedido.Status) ? (int)Enum.Parse<StatusEnum>(pedido.Status) : (int)StatusEnum.Cancelado);
            };

            await _channel.BasicConsumeAsync(queue: "pedidos-atualizados", autoAck: false, consumer: consumer);
        }

        public void Dispose()
        {
            _channel?.CloseAsync();
            _connection?.CloseAsync();
        }
    }
}
