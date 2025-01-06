using System.ComponentModel.DataAnnotations;

namespace MatchMaking.Infrastructure.Kafka.Configuration;

public sealed record KafkaTopicOptions
{
    [Required] public required string MatchCompleteEvents { get; init; }
}