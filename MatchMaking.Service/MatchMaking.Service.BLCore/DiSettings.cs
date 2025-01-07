using MatchMaking.Service.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Service;

public static class DiSettings
{
    public static IServiceCollection AddServices(this IServiceCollection services) =>
        services
            .AddScoped<IMatchMakingService, MatchMakingService>();
}