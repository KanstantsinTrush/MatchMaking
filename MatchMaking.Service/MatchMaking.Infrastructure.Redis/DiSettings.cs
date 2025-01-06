using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

namespace MatchMaking.Infrastructure.Redis;

public static class DiSettings
{
    public static IServiceCollection AddRedis(this IServiceCollection services, IHostEnvironment environment, IConfiguration configuration)
    {
        var redisConnectionString = RedisHelper.GetConnectionString(environment, configuration);
        var redisConnectionMultiplexer = ConnectionMultiplexer.Connect(redisConnectionString);
        services.AddSingleton<IConnectionMultiplexer>(redisConnectionMultiplexer);

        return services;
    }
}