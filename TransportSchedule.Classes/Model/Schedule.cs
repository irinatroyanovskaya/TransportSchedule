using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportScheduleClasses
{ 
    public class Schedule
    {
        public Station Station { get; set; }
        public Line Line { get; set; }
        public int Time { get; set; }
    }
}
