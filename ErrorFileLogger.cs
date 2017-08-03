using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

namespace ALARMS_x86
{
    class ErrorFileLogger : ILogger
    {
        public ErrorFileLogger()
        {

        }

        public void LogException(string errMsg)
        {
            StreamWriter errorlogfile = new StreamWriter("errorlog.txt");
            errorlogfile.WriteLine(errMsg);
            errorlogfile.Close();
        }
    }
}
