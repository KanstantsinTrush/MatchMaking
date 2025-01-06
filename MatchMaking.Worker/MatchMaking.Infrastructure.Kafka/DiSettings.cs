using MatchMaking.Infrastructure.Kafka.Consumers;
using MatchMaking.Infrastructure.Kafka.Producers;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Infrastructure.Kafka;

public static class DiSettings
{
    public static IServiceCollection AddKafkaProducerFactory(this IServiceCollection services) =>
        services
            .AddSingleton<IKafkaProducerFactory, KafkaProducerFactory>();
    
    public static IServiceCollection AddKafkaConsumerFactory(this IServiceCollection services) =>
        services
            .AddSingleton<IKafkaConsumerFactory, KafkaConsumerFactory>();
}