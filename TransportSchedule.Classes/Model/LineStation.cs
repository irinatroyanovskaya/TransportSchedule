using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Data.Entity;

namespace TransportScheduleClasses
{
    public class LineStation
    {
        public int Id { get; set; }
        //public int StationId { get; set; }
        //[ScriptIgnore]
        public Station Station { get; set; }
        //public int LineId { get; set; }
        //[ScriptIgnore]
        public Line Line { get; set; }
        public int TimeFromOrigin { get; set; }
    }
}
