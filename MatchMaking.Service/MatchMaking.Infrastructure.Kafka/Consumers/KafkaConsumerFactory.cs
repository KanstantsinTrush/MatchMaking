using Confluent.Kafka;
using MatchMaking.Infrastructure.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatchMaking.Infrastructure.Kafka.Consumers;

public class KafkaConsumerFactory(IOptions<KafkaConsumerOptions> consumerOptions) : IKafkaConsumerFactory
{
    private readonly ConsumerConfig _config = new()
    {
        GroupId = consumerOptions.Value.ConsumerGroup,
        AutoOffsetReset = AutoOffsetReset.Earliest,
        BootstrapServers = consumerOptions.Value.BootstrapServers,
        AllowAutoCreateTopics = true
    };

    public IConsumer<string, string> Create(string topic, ILogger logger)
    {
        var consumer = new ConsumerBuilder<string, string>(_config)
            .SetErrorHandler(ErrorHandler(logger, topic))
            .SetLogHandler(LogHandler(logger, topic))
            .Build();

        consumer.Subscribe(topic);
        return consumer;
    }

    private static Action<IConsumer<string, string>, Error> ErrorHandler(ILogger logger, string source) =>
        (_, error) =>
        {
            logger
                .LogError(
                    "Error consuming from {Source} in Kafka. Code - {Code}, Reason - {Reason}", source, error.Code,
                    error.Reason);
        };

    private static Action<IConsumer<string, string>, LogMessage> LogHandler(ILogger logger, string source) =>
        (_, message) =>
        {
            var logLevel = message.Level switch
            {
                SyslogLevel.Alert => LogLevel.Critical,
                SyslogLevel.Critical => LogLevel.Critical,
                SyslogLevel.Debug => LogLevel.Debug,
                SyslogLevel.Emergency => LogLevel.Critical,
                SyslogLevel.Error => LogLevel.Error,
                SyslogLevel.Info => LogLevel.Information,
                SyslogLevel.Notice => LogLevel.Information,
                SyslogLevel.Warning => LogLevel.Warning,
                _ => LogLevel.Warning
            };

            logger.Log(logLevel, "Log Kafka from source {Source}. Message - {Message}", source, message.Message);
        };
}