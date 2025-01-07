using System.Text.Json;
using Confluent.Kafka;
using MatchMaking.Infrastructure.Kafka.Configuration;
using MatchMaking.Infrastructure.Kafka.Producers;
using MatchMaking.Worker.BLCore.Models;
using MatchMaking.Worker.BLCore.Options;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using StackExchange.Redis;

namespace MatchMaking.Worker.BLCore.Services;

public class MatchMakingService(
    ILogger<MatchMakingService> logger,
    IKafkaProducerFactory producerFactory,
    IOptions<KafkaProducerOptions> producerOptions,
    IConnectionMultiplexer redisConnection,
    IOptions<MatchOptions> matchOptions) : IMatchMakingService
{
    private const int DefaultUsersPerMatchCount = 3;
    
    private readonly IDatabase _database = redisConnection.GetDatabase();
    private readonly int _usersPerMatchCount = matchOptions.Value.UsersPerMatchCount ?? DefaultUsersPerMatchCount;
    private readonly IProducer<string, string> _producer = producerFactory.Create<string, string>(logger);
    private readonly RedisKey _matchSetKey = new("kafka:match_set");
    
    public async Task ProcessUser(string userId)
    {
        var score = DateTime.UtcNow.Ticks;

        if (await _database.SortedSetAddAsync(_matchSetKey, userId, score))
        {
            logger.LogInformation("User is added to the queue.");

            return;
        }
        
        logger.LogInformation("User is already in queue.");
    }

    public async Task ProcessMatch()
    {
        var usersToMatch = await _database.SortedSetPopAsync(_matchSetKey, _usersPerMatchCount);
        if (usersToMatch.Length == _usersPerMatchCount)
        {
            var matchId = Guid.NewGuid().ToString();
            var players = usersToMatch.Select(p => p.Element.ToString()).ToArray();
            
            var match = new Match(matchId, players.ToList());
            await _producer.ProduceAsync(producerOptions.Value.MatchMakingCompleteEvents, new Message<string, string> { Value = JsonSerializer.Serialize(match) });
            
            logger.LogInformation("Match is created: {Users}", string.Join(", ", players));
        }
        else
        {
            await _database.SortedSetAddAsync(_matchSetKey, usersToMatch);
        }
    }
}