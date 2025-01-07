using Confluent.Kafka;
using Microsoft.Extensions.Logging;

namespace MatchMaking.Infrastructure.Kafka.Consumers;

public interface IKafkaConsumerFactory
{
    IConsumer<string, string> Create(string topic, ILogger logger);
}