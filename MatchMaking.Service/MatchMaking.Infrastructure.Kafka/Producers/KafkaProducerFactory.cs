using System.Collections.Concurrent;
using Confluent.Kafka;
using MatchMaking.Infrastructure.Kafka.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatchMaking.Infrastructure.Kafka.Producers;

internal sealed class KafkaProducerFactory(IOptions<KafkaProducerOptions> options) : IKafkaProducerFactory, IDisposable
{
    private readonly ConcurrentQueue<IDisposable> _producers = [];

    public IProducer<TKey, TValue> Create<TKey, TValue>(ILogger logger)
    {
        var config = CreateProducerConfig(options.Value);

        var logSource = logger.GetType().GetGenericArguments().FirstOrDefault()?.Name ?? "Kafka";
        var producer = new ProducerBuilder<TKey, TValue>(config)
            .SetErrorHandler(ErrorHandler<TKey, TValue>(logger, logSource))
            .SetLogHandler(LogHandler<TKey, TValue>(logger, logSource))
            .Build();
        _producers.Enqueue(producer);

        return producer;
    }

    private static ProducerConfig CreateProducerConfig(KafkaProducerOptions options) =>
        new()
        {
            BootstrapServers = options.BootstrapServers,
            AllowAutoCreateTopics = true,
            Acks = Acks.All
        };

    private static Action<IProducer<TKey, TValue>, Error> ErrorHandler<TKey, TValue>(ILogger logger, string source) =>
        (_, error) =>
        {
            logger
                .LogError(
                    "Error publishing from {Source} in Kafka. Code - {Code}, Reason - {Reason}", source, error.Code,
                    error.Reason);
        };

    private static Action<IProducer<TKey, TValue>, LogMessage> LogHandler<TKey, TValue>(ILogger logger, string source) =>
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

    public void Dispose()
    {
        while (_producers.TryDequeue(out var producer))
        {
            producer.Dispose();
        }
    }
}