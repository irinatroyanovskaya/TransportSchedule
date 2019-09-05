using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportScheduleClasses
{
    public class DBRepository: IRepository
    {
        public Context Context { get; set; } = new Context();
        public List<Schedule> Schedule { get; set; }

        public DBRepository()
        {
            Schedule = CreateSchedule(Context);
        }

        public static List<Schedule> CreateSchedule(Context context)
        {
            try
            {
                List<Schedule> schedule = new List<Schedule>();
                foreach (var line in context.Lines.ToList())
                {
                    foreach (var station in context.LineStation.Where(ls => ls.Line.Id == line.Id).Select(ls => ls.Station).ToList())
                    {
                        for (int k = 0; k < (line.LastDeparture - line.FirstDeparture) / line.Interval; k++)
                        {
                            AddAndSort(schedule,
                                CreateElementOfSchedule(
                                    line,
                                    station,
                                    line.FirstDeparture + context.LineStation.Where(ls => ls.Station.Id == station.Id && ls.Line.Id == line.Id).First().TimeFromOrigin + line.Interval * k,
                                    0));
                            AddAndSort(schedule,
                                CreateElementOfSchedule(
                                    line,
                                    station,
                                    line.FirstDeparture + context.LineStation.Where(ls => ls.Station.Id == station.Id && ls.Line.Id == line.Id).First().TimeFromOrigin + line.Interval * k,
                                    24 * 60));
                            AddAndSort(schedule,
                                CreateElementOfSchedule(
                                    line,
                                    station,
                                    line.FirstDeparture + context.LineStation.Where(ls => ls.Station.Id == station.Id && ls.Line.Id == line.Id).First().TimeFromOrigin + line.Interval * k,
                                    -24 * 60));
                        }
                    }
                }
                return schedule;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public static Schedule CreateElementOfSchedule(Line line, Station station, int time, int day)
        {
            Schedule elementOfSchedule = new Schedule()
            {
                Line = line,
                Station = station,
                Time = time + day,
            };
            return elementOfSchedule;
        }

        public List<TableMainWindow> CalculateTime(Station request)
        {
            if (request == null)
                return new List<TableMainWindow>();

            List<TableMainWindow> scheduleTable = new List<TableMainWindow>();

            foreach (var line in Context.Lines)
                line.Chosen = false;

            foreach (Schedule elementofschedule in Schedule)
            {
                if (request.Name == elementofschedule.Station.Name &&
                    request.Name != elementofschedule.Line.Destination.Name &&
                    elementofschedule.Time - (DateTime.Now.Hour * 60 + DateTime.Now.Minute) >= 0 &&
                    elementofschedule.Line.Chosen == false)
                {
                    TableMainWindow row = new TableMainWindow()
                    {
                        Line = elementofschedule.Line.Number,
                        Destination = elementofschedule.Line.Destination.Name,
                        Time = (elementofschedule.Time - (DateTime.Now.Hour * 60 + DateTime.Now.Minute)).ToString(),
                    };
                    if (int.Parse(row.Time) >= 60)
                        row.Time = (int.Parse(row.Time) / 60).ToString() + "H " + (int.Parse(row.Time) % 60).ToString() + " minutes";
                    else
                        row.Time = row.Time + " minutes";
                    scheduleTable.Add(row);
                    elementofschedule.Line.Chosen = true;
                }
            }
            return scheduleTable;
        }

        public void CreateNewUser(string name, string email, string password)
        {
            try
            {
                if (IsEmailUnique(email))
                {
                    Context.Users.Add(new User
                    {
                        Name = name,
                        Email = email,
                        PasswordHash = User.GetHash(password),
                        FavouriteStations = new List<Favourite>()
                    }
                    );
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
        }

        public bool IsEmailUnique(string email)
        {
            try
            {
                if (!Context.Users.Any(u => u.Email == email))
                    return true;
                return false;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return false;
            }
        }

        public bool AreThereAnyUsers()
        {
            try
            {
                if (Context.Users.Count() == 0)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return false;
            }
        }

        public bool IsFavouriteStationUnique(User user, Station station)
        {
            try
            {
                if (user.FavouriteStations.Any(f => f.Station.Id == station.Id))
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return false;
            }
        }

        public User AutentificateUser(string email, string passwordhash)
        {
            try
            {
                if (Context.Users.Any(u => u.Email == email && u.PasswordHash == passwordhash))
                {
                    var user = Context.Users.First(u => u.Email == email && u.PasswordHash == passwordhash);
                    if (user.FavouriteStations == null)
                        user.FavouriteStations = new List<Favourite>();
                    return user;
                }
                else
                    return null;
            }
            catch(Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public List<Line> ShowOnlyDirectLines(List<Line> lines)
        {
            List<Line> onlyDirectLines = new List<Line>();
            for (int i = 0; i < (lines.Count) / 2; i++)
                onlyDirectLines.Add(lines[i]);
            return onlyDirectLines;
        }

        public void AddFavourite(User user, Station station, string description)
        {
            try
            {
                if (!Context.Users.First(u => u.Id == user.Id).FavouriteStations.Any(f => f.Station.Id == station.Id))
                {
                    var favourite = new Favourite { Description = description, Station = Context.Stations.First(st => st.Id == station.Id), User = Context.Users.First(u => u.Id == user.Id) };
                    Context.Favourites.Add(favourite);
                    Context.Users.First(u => u.Id == user.Id).FavouriteStations.Add(favourite);
                    Context.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
        }

        public void DeleteFavourite(User user, Favourite favourite)
        {
            try
            {
                Context.Users.First(u => u.Id == user.Id).FavouriteStations.Remove(favourite);
                Context.Favourites.Remove(favourite);
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
        }

        public void EditFavourite(Favourite favourite, string description, User user)
        {
            try
            {
                Context.Users.First(u => u.Id == user.Id).FavouriteStations.First(f => f.Id == favourite.Id).Description = description;
                Context.Favourites.First(f => f.Id == favourite.Id).Description = description;
                Context.SaveChanges();
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
            }
        }

        public List<Station> ShowStationsInLine(Line line)
        {
            try
            {
                List<Station> stationsInLine = new List<Station>();
                foreach (var lineStation in Context.LineStation.Where(ls => ls.Line.Id == line.Id))
                    stationsInLine.Add(lineStation.Station);
                return stationsInLine;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public static void AddAndSort(List<Schedule> list, Schedule element) //метод добавляет элемент расписания в список так, чтобы элементы списка были отсортированы по времени
        {
            if (list.Count == 0 || element.Time >= list[list.Count - 1].Time)
            {
                list.Add(element);
                return;
            }
            if (element.Time <= list[0].Time)
            {
                list.Insert(0, element);
                return;
            }
            int firstElement = 0;
            int lastElement = list.Count - 1;
            int averageElement;
            do
            {
                averageElement = (lastElement + firstElement) / 2;
                if (lastElement - firstElement == 1 || element.Time == list[lastElement].Time)
                {
                    list.Insert(lastElement, element);
                    return;
                }
                if (element.Time == list[firstElement].Time)
                {
                    list.Insert(firstElement, element);
                    return;
                }
                if (element.Time >= list[averageElement].Time)
                {
                    firstElement = averageElement;
                }
                if (element.Time <= list[averageElement].Time)
                {
                    lastElement = averageElement;
                }
            } while (true);
        }
    }
}
