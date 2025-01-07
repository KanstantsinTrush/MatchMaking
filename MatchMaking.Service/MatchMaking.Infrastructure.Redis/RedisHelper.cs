using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace MatchMaking.Infrastructure.Redis;

public static class RedisHelper
{
    public static string GetConnectionString(IHostEnvironment environment, IConfiguration configuration)
    {
        var redisHost = configuration.GetValue<string>("REDIS_HOST");
        var redisPort = configuration.GetValue<string>("REDIS_PORT");

        var builder = ConfigurationOptions.Parse($"{redisHost}:{redisPort}");

        return builder.ToString();
    }
}
