using BCCIAwbApi.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;

namespace BCCIAwbApi.Data
{
    public class CricketDbContext : DbContext
    {
        public CricketDbContext(DbContextOptions<CricketDbContext> options) : base(options) { }
        public DbSet<Series> Series { get; set; }
        public DbSet<Match> Matches { get; set; }
    }
}
