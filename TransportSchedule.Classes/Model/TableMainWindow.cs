using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransportScheduleClasses
{
    public class TableMainWindow //класс для отображения расписания в виде таблицы в главном окне
    {
        public int Line { get; set; }
        public string Destination { get; set; }
        public string Time { get; set; }
    }
}
