using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Infrastructure.Kafka.Configuration;

public class KafkaConsumerOptions
{
    [Required]
    public required string BootstrapServers { get; set; }
    [Required]
    public required string ConsumerGroup { get; set; }
    [Required]
    public required string MatchMakingCompleteEvents { get; set; }
}