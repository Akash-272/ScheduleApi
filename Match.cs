public class Match
{
    public int MatchID { get; set; }
    public int SeriesID { get; set; }
    public string MatchDate { get; set; }
    public string Venue { get; set; }
    public string Team1 { get; set; }
    public string Team2 { get; set; }

    // Parameterless constructor
    public Match() { }

    // Constructor matching the signature
    public Match(int matchID, int seriesID, string matchDate, string venue, string team1, string team2)
    {
        MatchID = matchID;
        SeriesID = seriesID;
        MatchDate = matchDate;
        Venue = venue;
        Team1 = team1;
        Team2 = team2;
    }
}
