using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ALARMS_x86
{
    public class LogicalDevice : Query
    {
        public string DeviceID { get; set; }
        public string Devicename { get; set; }
        public string timeused { get; set; }
        public string dateoflastservice { get; set; }

    }
}
