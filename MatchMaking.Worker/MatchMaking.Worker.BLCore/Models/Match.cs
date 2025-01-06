namespace MatchMaking.Worker.BLCore.Models;

public record Match(
    string MatchId,
    List<string> UserIds);