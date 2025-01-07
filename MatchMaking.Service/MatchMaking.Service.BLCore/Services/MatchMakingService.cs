using Confluent.Kafka;
using MatchMaking.Infrastructure.Kafka.Configuration;
using MatchMaking.Infrastructure.Kafka.Producers;
using MatchMaking.Service.DBCore.Repository;
using MatchMaking.Service.DBEntites;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace MatchMaking.Service.Services;

public class MatchMakingService(
    ILogger<MatchMakingService> logger,
    IKafkaProducerFactory kafkaProducerFactory,
    IMatchMakingRepository matchMakingRepository,
    IOptions<KafkaProducerOptions> options) : IMatchMakingService
{
    private readonly IProducer<string, string> _producer = kafkaProducerFactory.Create<string, string>(logger);
    
    public async Task Request(Guid userId, CancellationToken cancellationToken)
    {
        var result = await _producer
            .ProduceAsync(
                options.Value.MatchMakingRequestEvents,
                new Message<string, string>
                {
                    Value = userId.ToString()
                },
                cancellationToken);

        if (result.Status is PersistenceStatus.NotPersisted)
            logger.LogWarning("Cannot send request for user: {UserId}", userId);
    }

    public Task ProcessMatch(Match match) => matchMakingRepository.SaveMatchForUsersAsync(match);

    public async Task<Match?> GetMatchForUser(Guid userId)
    {
        var match = await matchMakingRepository.GetMatchForUserAsync(userId);

        if (match == null)
        {
            logger.LogInformation("No match found for user {UserId}.", userId);
            return null;
        }

        logger.LogInformation("Match found for user {UserId}: {MatchId}", userId, match.MatchId);
        return match;
    }
}