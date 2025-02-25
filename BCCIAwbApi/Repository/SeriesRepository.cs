using BCCIAwbApi.Data;
using BCCIAwbApi.Models;
using Microsoft.EntityFrameworkCore;

namespace BCCIAwbApi.Repository
{
    public class SeriesRepository : ISeriesRepository
    {
        private readonly CricketDbContext _context;

        public SeriesRepository(CricketDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public void AddSeries(Series series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            _context.Series.Add(series);
            _context.SaveChanges();
        }

        public void UpdateSeries(Series series)
        {
            if (series == null) throw new ArgumentNullException(nameof(series));
            _context.Series.Update(series);
            _context.SaveChanges();
        }

        public void DeleteSeries(int id)
        {
            var series = _context.Series.Find(id);
            if (series != null)
            {
                _context.Series.Remove(series);
                _context.SaveChanges();
            }
        }

        public Series GetSeriesById(int id)
        {
            return _context.Series.Include(s => s.Matches).FirstOrDefault(s => s.SeriesID == id);
        }

        public List<Series> GetAllSeries()
        {
            return _context.Series.Include(s => s.Matches).ToList();
        }
    }
}
