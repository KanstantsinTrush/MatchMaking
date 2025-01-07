namespace MatchMaking.Service.DBEntites;

public record Match(
    string MatchId,
    List<string> UserIds);