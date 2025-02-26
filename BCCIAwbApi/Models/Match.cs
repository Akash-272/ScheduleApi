namespace BCCIAwbApi.Models
{
    public class Match
    {
        public int MatchID { get; set; }
        public int SeriesID { get; set; }
        public string MatchDate { get; set; }
        public string Venue { get; set; }
        public string Team1 { get; set; }
        public string Team2 { get; set; }

    }
}
