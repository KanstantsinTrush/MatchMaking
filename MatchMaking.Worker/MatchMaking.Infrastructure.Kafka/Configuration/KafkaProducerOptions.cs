using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Infrastructure.Kafka.Configuration;

public sealed class KafkaProducerOptions
{
    [Required]
    public required string BootstrapServers { get; set; }
    [Required]
    public required string MatchMakingCompleteEvents { get; init; }
}