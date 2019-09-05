using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportScheduleClasses
{
    public class Context : DbContext
    {
        public DbSet<Station> Stations { get; set; }
        public DbSet<Line> Lines { get; set; }
        public DbSet<LineStation> LineStation { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Favourite> Favourites { get; set; }
        //public DbSet<Schedule> Schedule { get; set; }

        public Context(): base("localsql")
        { }
    }
}
