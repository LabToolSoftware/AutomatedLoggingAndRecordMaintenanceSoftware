using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using System.Diagnostics;


namespace ALARMS_x86
{
    class ErrorManager
    {

        private string _callerClass;
        private string _callerMethod;

        public ErrorManager()
        {

            StackTrace st = new StackTrace();
            this._callerClass = st.GetFrame(1).GetMethod().ReflectedType.ToString();
            this._callerMethod = st.GetFrame(1).GetMethod().Name;
        }

        public void LogException(Exception e)
        {
            QueryManager qMgr = new QueryManager();
            ErrorFileLogger filelogger = new ErrorFileLogger();
            string exceptionMsg = UserReadableForm(e);
            qMgr.LogException(exceptionMsg);
            filelogger.LogException(exceptionMsg);
        }

        private string UserReadableForm(Exception e)
        {
            return "'" + DateTime.Now.ToString() + "','" + _callerClass + "','" + _callerMethod + "','" + e.GetType() + "','" + e.Message.Split('\n')[0] + "'";
        }
    }
}
