using MatchMaking.Service.DBCore.Repository;
using Microsoft.Extensions.DependencyInjection;

namespace MatchMaking.Service.DBCore;

public static class DiSettings
{
    public static IServiceCollection AddRepositories(this IServiceCollection services) =>
        services
            .AddScoped<IMatchMakingRepository, MatchMakingRepository>();
}