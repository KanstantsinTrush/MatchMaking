using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Infrastructure.Kafka.Configuration;

public class KafkaConsumerOptions
{
    [Required]
    public string? BootstrapServers { get; set; }
    [Required]
    public string? ConsumerGroup { get; set; }
    [Required]
    public required string MatchMakingRequestEvents { get; init; }
}