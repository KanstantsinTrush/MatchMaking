using MatchMaking.Service.DBEntites;

namespace MatchMaking.Service.Services;

public interface IMatchMakingService
{
    Task Request(Guid userId, CancellationToken cancellationToken);
    Task ProcessMatch(Match match);
    Task<Match?> GetMatchForUser(Guid userId);
}