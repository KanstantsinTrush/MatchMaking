namespace MatchMaking.Worker.BLCore.Services;

public interface IMatchMakingService
{
    Task ProcessUser(string userId);
    Task ProcessMatch();
}