using MatchMaking.Service.DBEntites;

namespace MatchMaking.Service.DBCore.Repository;

public interface IMatchMakingRepository
{
    public Task SaveMatchForUsersAsync(Match match);

    public Task<Match?> GetMatchForUserAsync(Guid userId);
}