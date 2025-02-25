using BCCIAwbApi.Models;

namespace BCCIAwbApi.Repository
{
    public interface ISeriesRepository
    {
        void AddSeries(Series series);
        void UpdateSeries(Series series);
        void DeleteSeries(int id);
        Series GetSeriesById(int id);
        List<Series> GetAllSeries();
    }
}
