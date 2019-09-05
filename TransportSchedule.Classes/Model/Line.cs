using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TransportScheduleClasses
{
    public class Line
    {
        public int Id { get; set; }
        public int Number { get; set; }
        public int Interval { get; set; }
        public int FirstDeparture { get; set; }
        public int LastDeparture { get; set; }
        //[ScriptIgnore]
        public Station Destination { get; set; }
        //public int DestinationId { get; set; }
        //[ScriptIgnore]
        //public List<Station> Stations { get; set; }
        //public List<int> StationsId { get; set; }
        //public List<int> IntervalsBetweenStations { get; set; }
        [ScriptIgnore]
        public bool Chosen { get; set; }
    }
}
