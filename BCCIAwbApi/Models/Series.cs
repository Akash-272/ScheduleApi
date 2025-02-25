namespace BCCIAwbApi.Models
{
    public class Series
    {
        public int SeriesID { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<Match> Matches { get; set; } = new List<Match>();
    }
}
