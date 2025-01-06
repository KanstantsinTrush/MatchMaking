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
        var redisPassword = configuration.GetValue<string>("REDIS_PASSWORD");

        var builder = ConfigurationOptions.Parse($"{redisHost}:{redisPort}");

        if (!environment.IsDevelopment())
        {
            builder.Password = redisPassword;
        }

        return builder.ToString();
    }
}
