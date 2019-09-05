namespace TransportScheduleClasses.Migrations
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Migrations;
    using System.Linq;

    internal sealed class Configuration : DbMigrationsConfiguration<TransportScheduleClasses.Context>
    {
        public Configuration()
        {
            AutomaticMigrationsEnabled = true;
        }

        protected override void Seed(Context context)
        {
            //  This method will be called after migrating to the latest version.

            //  You can use the DbSet<T>.AddOrUpdate() helper extension method 
            //  to avoid creating duplicate seed data.
            FileRepository repo = new FileRepository();

            foreach (var station in repo.Stations)
            {
                context.Stations.AddOrUpdate(station);
                context.SaveChanges();
            }

            foreach (var line in repo.Lines)
            {
                line.Destination = context.Stations.Where(st => st.Name == line.Destination.Name).First();
                context.Lines.AddOrUpdate(line);
                context.SaveChanges();
            }

            foreach (var user in repo.Users)
            {
                foreach (var favourite in user.FavouriteStations)
                {
                    favourite.Station = context.Stations.Where(st => st.Name == favourite.Station.Name).First();
                }
                context.Users.AddOrUpdate(user);
                context.SaveChanges();
            }


            foreach (var linestation in repo.LinesStations)
            {
                linestation.Station = context.Stations.Where(st => st.Name == linestation.Station.Name).First();
                linestation.Line = context.Lines.Where(l => l.Number == linestation.Line.Number && l.Destination.Id == linestation.Line.Destination.Id).First();
                context.LineStation.AddOrUpdate(linestation);
                context.SaveChanges();
            }
        }
    }
}
