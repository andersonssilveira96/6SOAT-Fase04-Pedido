using Domain.Producer;
using Microsoft.Extensions.DependencyInjection;

namespace Infra.MessageBroker
{
    public static class InfraMessageBrokerExtensions
    {
        public static IServiceCollection AddInfraMessageBrokerServices(this IServiceCollection services)
        {
            services.AddScoped<IMessageBrokerProducer, MessageBrokerProducer>();
            return services;
        }
    }
}
