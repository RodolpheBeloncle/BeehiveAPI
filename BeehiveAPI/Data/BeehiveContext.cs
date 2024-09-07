// Data/BeehiveContext.cs
using Microsoft.EntityFrameworkCore;
using BeehiveAPI.Models;

namespace BeehiveAPI.Data
{
    public class BeehiveContext : DbContext
    {
        public BeehiveContext(DbContextOptions<BeehiveContext> options) : base(options) { }

        public DbSet<Beehive> Beehives { get; set; }
        public DbSet<Sensor> Sensors { get; set; }
    }
}
