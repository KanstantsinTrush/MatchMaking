using System.Text.Json;
using MatchMaking.Service.DBEntites;
using StackExchange.Redis;

namespace MatchMaking.Service.DBCore.Repository;

public class MatchMakingRepository(IConnectionMultiplexer redisConnection) : IMatchMakingRepository
{
    private readonly IDatabase _redisDb = redisConnection.GetDatabase();
    private const string MatchKeyTemplate = "user:{0}:match";

    public async Task SaveMatchForUsersAsync(Match match)
    {
        var serializedMatch = JsonSerializer.Serialize(match);

        foreach (var userId in match.UserIds)
        {
            var key = new RedisKey(string.Format(MatchKeyTemplate, userId));
            await _redisDb.StringSetAsync(key, serializedMatch);
        }
    }

    public async Task<Match?> GetMatchForUserAsync(Guid userId)
    {
        var key = new RedisKey(string.Format(MatchKeyTemplate, userId));
        var matchJson = await _redisDb.StringGetAsync(key);

        if (matchJson.IsNullOrEmpty)
            return null;

        return JsonSerializer.Deserialize<Match>(matchJson);
    }
}