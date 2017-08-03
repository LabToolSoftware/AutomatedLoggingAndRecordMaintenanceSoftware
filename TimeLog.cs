using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace ALARMS_x86
{
    public abstract class Query
    {

    }

    public class TimeLog : Query
    {
        public string DeviceID { get; set; }
        public string DeviceName { get; set; }
        public string UserName { get; set; }
        public string DateUsed { get; set; }
        public string TimeOn { get; set; }
        public string TimeOff { get; set; }
        public string TimeUsed { get; set; }

    }
}
