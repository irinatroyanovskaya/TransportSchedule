using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TransportScheduleClasses
{
    public class FileRepository: IRepository
    {
        const string filename = "Data.json";
        public List<Schedule> Schedule { get; set; }
        public List<Station> Stations { get; set; }
        public List<Line> Lines { get; set; }
        public List<LineStation> LinesStations { get; set; }
        public List<User> Users { get; set; }

        public FileRepository()
        {
            Stations = DeserializeStations(new StreamReader(filename));
            Lines = DeserializeLines(new StreamReader(filename));
            LinesStations = DeserializeLineStation(new StreamReader(filename));
            Schedule = CreateSchedule(Lines, LinesStations);
            Users = DeserializeUsers();
        }

        public static List<Station> DeserializeStations(StreamReader sr)
        {
            try
            {
                var deserializer = new JavaScriptSerializer();
                var jsonStations = deserializer.Deserialize<List<Station>>(sr.ReadLine());
                return jsonStations;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public static List<Line> DeserializeLines(StreamReader sr)
        {
            List<Station> stations = DeserializeStations(sr);
            try
            {
                var deserializer = new JavaScriptSerializer();
                var jsonLines = deserializer.Deserialize<List<Line>>(sr.ReadLine());
                List<Line> lines = new List<Line>();
                foreach (Line line in jsonLines)
                {
                    foreach (var station in stations)
                    {
                        if (line.Destination.Id == station.Id)
                        {
                            line.Destination = station;
                            break;
                        }
                    }
                    lines.Add(line);
                }
                return lines;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public static List<LineStation> DeserializeLineStation(StreamReader sr)
        {
            List<Line> lines = DeserializeLines(sr);
            try
            {
                var deserializer = new JavaScriptSerializer();
                var linesStations = deserializer.Deserialize<List<LineStation>>(sr.ReadLine());
                foreach (var lineStation in linesStations)
                {
                    foreach (var line in lines)
                    {
                        if (lineStation.Line.Id == line.Id)
                        {
                            lineStation.Line = line;
                            foreach (var station in DeserializeStations(new StreamReader(filename)))
                            {
                                if (lineStation.Station.Id == station.Id)
                                {
                                    lineStation.Station = station;
                                    break;
                                }
                            }
                            break;
                        }
                    }
                }
                return linesStations;
            }
            catch (Exception ex)
            {
                ex.GetBaseException();
                return null;
            }
        }

        public static List<User> DeserializeUsers()
        {
            using (var sr = new StreamReader(filename))
            {
                DeserializeLineStation(sr);
                var deserializer = new JavaScriptSerializer();
                try
                {
                    var jsonUsers = deserializer.Deserialize<List<User>>(sr.ReadLine());
                    if (jsonUsers == null)
                        return new List<User>();
                    foreach (User user in jsonUsers)
                    {
                        if (user.FavouriteStations == null)
                            user.FavouriteStations = new List<Favourite>();
                        else
                        {
                            foreach (var favourite in user.FavouriteStations)
                            {
                                foreach (Station station in DeserializeStations(new StreamReader(filename)))
                                {
                                    if (favourite.Station.Id == station.Id)
                                    {
                                        favourite.Station = station;
                                    }
                                }
                            }
                        }
                    }
                    return jsonUsers;
                }
                catch (Exception ex)
                {
                    ex.GetBaseException();
                    return null;
                }
            }
        }

        public void SerializeData(List<Station> stations, List<Line> lines, List<User> users, List<LineStation> linesStations)
        {
            using (var sw = new StreamWriter(filename))
            {
                var serializer = new JavaScriptSerializer();
                var jsonStations = serializer.Serialize(stations);
                sw.WriteLine(jsonStations);
                var jsonLines = serializer.Serialize(lines);
                sw.WriteLine(jsonLines);
                var jsonLinesStations = serializer.Serialize(linesStations);
                sw.WriteLine(jsonLinesStations);
                var jsonUsers = serializer.Serialize(users);
                sw.WriteLine(jsonUsers);
            }
        }

        public void CreateNewUser(string name, string email, string password)
        {
            var user = new User()
            {
                Name = name,
                Email = email,
                PasswordHash = User.GetHash(password)
            };
            Users.Add(user);
            SerializeData(Stations, ShowOnlyDirectLines(Lines), Users, LinesStations);
        }

        public List<Line> ShowOnlyDirectLines(List<Line> lines)
        {
            List<Line> onlyDirectLines = new List<Line>();
            for (int i=0; i<(lines.Count)/2; i++)
                onlyDirectLines.Add(lines[i]);
            return onlyDirectLines;
        }

        public List<Schedule> CreateSchedule(List<Line> lines, List<LineStation> linesStations)
        {
            List<Schedule> schedule = new List<Schedule>();
            for (int i = 0; i < lines.Count; i++)
            {
                foreach (var lineStation in linesStations.Where(ls => ls.Line.Id==lines[i].Id))
                {
                    for (int k = 0; k < (lines[i].LastDeparture - lines[i].FirstDeparture) / lines[i].Interval; k++)
                    {
                        AddAndSort(schedule, CreateElementOfSchedule(lines[i], lineStation.Station, lines[i].FirstDeparture + lineStation.TimeFromOrigin + lines[i].Interval * k, 0));
                        AddAndSort(schedule, CreateElementOfSchedule(lines[i], lineStation.Station, lines[i].FirstDeparture + lineStation.TimeFromOrigin + lines[i].Interval * k, 24*60));
                        AddAndSort(schedule, CreateElementOfSchedule(lines[i], lineStation.Station, lines[i].FirstDeparture + lineStation.TimeFromOrigin + lines[i].Interval * k, -24*60));
                    }
                }
            }
            return schedule;
        }

        public Schedule CreateElementOfSchedule(Line line, Station station, int time, int day)
        {
            Schedule elementOfSchedule = new Schedule()
            {
                Line = line,
                Station = station,
                Time = time + day,
            };
            return elementOfSchedule;
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

        public List<TableMainWindow> CalculateTime(Station request)
        {
            if (request == null)
                return new List<TableMainWindow>();

            List<TableMainWindow> scheduleTable = new List<TableMainWindow>();

            foreach (var line in Lines)
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

        public void AddFavourite(User user, Station station, string description)
        {
            user.FavouriteStations.Add(new Favourite { Station = station, Description = description});
            SerializeData(Stations, Lines, Users, LinesStations);
        }

        public void EditFavourite(Favourite favourite, string description, User user)
        {
            user.FavouriteStations.First(f => f.Id == favourite.Id).Description = description;
            SerializeData(Stations, Lines, Users, LinesStations);
        }

        public void DeleteFavourite(User user, Favourite favourite)
        {
            user.FavouriteStations.Remove(favourite);
            SerializeData(Stations, Lines, Users, LinesStations);
        }

        public List<Station> ShowStationsInLine(Line line)
        {
            List<Station> stations = new List<Station>();
            foreach (var lineStation in LinesStations.Where(ls => ls.Line.Id == line.Id))
                stations.Add(lineStation.Station);
            return stations;
        }
    }
}
