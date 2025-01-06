namespace MatchMaking.Service.DBEntites;

public class Match
{
    public string MatchId { get; set; }
    public List<string> UserIds { get; set; }
}