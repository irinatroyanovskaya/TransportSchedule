using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace TransportScheduleClasses
{
    public class Favourite
    {
        public int Id { get; set; }
        public Station Station { get; set; }
        public string Description { get; set; }
        public User User { get; set; }
    }
}
