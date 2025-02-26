namespace BCCIAwbApi.Models
{
    public class Schedule
    {
        public int ScheduleID { get; set; }
        public int SeriesID { get; set; }
        public int MatchID { get; set; }
        public string ScheduledDate { get; set; }

        public Series Series { get; set; }
        public Match Match { get; set; }
    }
}
