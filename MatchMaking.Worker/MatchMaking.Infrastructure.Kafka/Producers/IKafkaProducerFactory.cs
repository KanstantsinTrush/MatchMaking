using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Infrastructure.Kafka.Producers;

public interface IKafkaProducerFactory
{
    IProducer<TKey, TValue> Create<TKey, TValue>(ILogger logger);
}