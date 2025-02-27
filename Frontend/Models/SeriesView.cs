
namespace Frontend.Models
{
    public class SeriesView
    {
        public int SeriesID { get; set; }
        public string Name { get; set; }
        public string StartDate { get; set; }
        public string EndDate { get; set; }
        public List<MatchView> Matches { get; set; } = new List<MatchView>();
    }
}
