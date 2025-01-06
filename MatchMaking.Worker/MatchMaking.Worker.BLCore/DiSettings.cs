using MatchMaking.Worker.BLCore.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Worker.BLCore;

public static class DiSettings
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IMatchMakingService, MatchMakingService>();
}