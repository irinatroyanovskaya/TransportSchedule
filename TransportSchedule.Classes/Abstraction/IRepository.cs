using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportScheduleClasses
{
    interface IRepository
    {
        List<TableMainWindow> CalculateTime(Station request);

        List<Line> ShowOnlyDirectLines(List<Line> lines);

        List<Station> ShowStationsInLine(Line line);

        void CreateNewUser(string name, string email, string password);

        void AddFavourite(User user, Station station, string description);

        void DeleteFavourite(User user, Favourite favourite);

        void EditFavourite(Favourite favourite, string description, User user);

    }
}
