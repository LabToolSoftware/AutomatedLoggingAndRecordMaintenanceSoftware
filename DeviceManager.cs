using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Management;
using System.Windows.Forms;
using System.Data;

namespace ALARMS_x86
{
    class DeviceManager
    {

        private bool firstcall = true;
        private UserDetails user;
        private TimeLog log;
        private static DeviceManager _instance;

        public List<string> DeviceIDToMon = new List<string>();
        public List<string> DeviceNameToMon = new List<string>();

        private string DeviceID;
        private string DeviceName;
        private string UserName;
        private string DateUsed;
        private DateTime TimeOn;
        private DateTime TimeOff;
        private string TimeUsed;

        public DeviceManager()
        {
            DevToMonitor();
            MonitorSystemDeviceChanges();
            MonitorSerialPortChanges();
        }
        public static DeviceManager GetDeviceMgr()
        {
            if (_instance == null)
            {
                _instance = new DeviceManager();
            }

            return _instance;
        }
        private void DevToMonitor()
        {
            QueryManager qMgr = new QueryManager();
            DataTable returnedrows_devices = qMgr.RetrieveData("Instruments");
            if (returnedrows_devices != null)
            {
                foreach (DataRow row in returnedrows_devices.Rows)
                {
                    DeviceIDToMon.Add(row["DeviceID"].ToString());
                    DeviceNameToMon.Add(row["DeviceName"].ToString());
                }
            }


        }
        public List<string> GetSysDevList()
        {
            WqlObjectQuery query = new WqlObjectQuery("SELECT * FROM CIM_LogicalDevice");
            using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
            {
                List<string> devList = new List<string>();
                ManagementObjectCollection collection;
                collection = searcher.Get();
                foreach (ManagementBaseObject mbo in collection)
                {
                    devList.Add(mbo["DeviceID"].ToString());
                }
                collection.Dispose();
                return devList;
            }
        }
        public List<string> GetSysDevList(string DeviceID)
        {
            string editedDeviceID = "";
            if (DeviceID.Contains(@"\"))
            {
                editedDeviceID = DeviceID.Replace("\\", "\\\\");
                MessageBox.Show(editedDeviceID);
            }
            else
            {
                editedDeviceID = DeviceID;
            }
            WqlObjectQuery query = new WqlObjectQuery("SELECT * FROM CIM_LogicalDevice WHERE DeviceID = '" + editedDeviceID + "'");
            try
            {
                using (ManagementObjectSearcher searcher = new ManagementObjectSearcher(query))
                {
                    List<string> devList = new List<string>();
                    ManagementObjectCollection collection;
                    collection = searcher.Get();
                    foreach (ManagementBaseObject mbo in collection)
                    {
                        devList.Add(mbo["DeviceID"].ToString());
                    }
                    collection.Dispose();
                    return devList;
                }
            }
            catch (Exception ex)
            {

                MessageBox.Show(ex.Message);
                return new List<string>();
            }

        }

        private void MonitorSystemDeviceChanges()
        {
            var connected = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 2");
            var disconnected = new WqlEventQuery("SELECT * FROM Win32_DeviceChangeEvent WHERE EventType = 3");

            ManagementEventWatcher connected_EventArrived = new ManagementEventWatcher(connected);
            ManagementEventWatcher disconnected_EventArrived = new ManagementEventWatcher(disconnected);

            connected_EventArrived.EventArrived += connected_EventArrivedHandler;
            disconnected_EventArrived.EventArrived += disconnected_EventArrivedHandler;

            connected_EventArrived.Start();
            disconnected_EventArrived.Start();

        }

        private void MonitorSerialPortChanges()
        {
            var serialportchangedstatus = new WqlEventQuery("SELECT * FROM __InstanceModificationEvent WHERE TargeInstance ISA 'CIM_LogicalDevice' AND TargetInstance.Name = 'COM1'");

            ManagementEventWatcher serialstatuschange_Event = new ManagementEventWatcher(serialportchangedstatus);

            serialstatuschange_Event.EventArrived += serialportstatuschanged_EventHandler;

            serialstatuschange_Event.Start();
        }
        public void UserNameEntered(string username)
        {
            this.UserName = username;
        }
        private void CalcTimeUsed()
        {
            TimeSpan timeelapsed = this.TimeOff.Subtract(this.TimeOn);
            this.TimeUsed = timeelapsed.ToString();
        }
        private void MakeNewTimeStamp()
        {
            log = new TimeLog();
            QueryManager qMgr = new QueryManager();

            log.DeviceID = this.DeviceID;
            log.DeviceName = this.DeviceName;
            log.UserName = this.UserName;
            log.DateUsed = this.DateUsed;
            log.TimeOn = this.TimeOn.ToShortTimeString();
            log.TimeOff = this.TimeOff.ToShortTimeString();
            log.TimeUsed = this.TimeUsed;

            qMgr.InsertRow("TimeLog", log);

            MessageBox.Show(this.DeviceName + " used for : " + TimeUsed);
        }
        private void MakeNewUserDetailsUIForm()
        {
            user = new UserDetails();
            user.ShowDialog();
        }
        private void CalcTotalDevUsageTime(string deviceID)
        {
            QueryManager qMgr = new QueryManager();
            DataTable instrument_details = qMgr.RetrieveData("Instruments", "DeviceID", deviceID);
            DataTable timelogs = qMgr.RetrieveData("Timelog", "DeviceID", deviceID);
            TimeSpan TotalTimeUsed = new TimeSpan();
            foreach (DataRow instrument in instrument_details.Rows)
            {
                foreach (DataRow row in timelogs.Rows)
                {
                    if (Convert.ToDateTime(row["DateUsed"].ToString()) >= Convert.ToDateTime(instrument["DateofLastService"].ToString()))
                    {
                        TotalTimeUsed += TimeSpan.Parse(row["TimeUsed"].ToString());

                    }
                }
                MessageBox.Show(instrument["DeviceName"].ToString() + " has been used for a total of " + TotalTimeUsed.Hours + " hour(s) and " + TotalTimeUsed.Minutes + " min");
            }



        }
        public void OnNewDevAddedToRegEventHandler(object source, EventArgs e)
        {
            DevToMonitor();
        }
        public void connected_EventArrivedHandler(object sender, EventArrivedEventArgs e)
        {
            if (firstcall == true)
            {
                firstcall = false;
                List<string> devList = new List<string>();
                foreach (var item in DeviceIDToMon)
                {
                    devList = GetSysDevList(item);
                    if (devList.Contains(item))
                    {
                        CalcTotalDevUsageTime("'" + item + "'");
                        MakeNewUserDetailsUIForm();
                        this.DeviceID = item;
                        this.DeviceName = DeviceNameToMon[DeviceIDToMon.IndexOf(item)];
                        this.DateUsed = DateTime.Now.Date.ToShortDateString();
                        this.TimeOn = DateTime.FromBinary(Int64.Parse(e.NewEvent.GetPropertyValue("TIME_CREATED").ToString())).ToLocalTime();
                    }
                }
            }
        }
        public void disconnected_EventArrivedHandler(object sender, EventArrivedEventArgs e)
        {
            if (firstcall == false)
            {
                firstcall = true;
                List<string> devList = GetSysDevList();

                if (!string.IsNullOrEmpty(this.DeviceID))
                {
                    if (!devList.Contains(this.DeviceID))
                    {
                        this.TimeOff = DateTime.FromBinary(Int64.Parse(e.NewEvent.GetPropertyValue("TIME_CREATED").ToString())).ToLocalTime();
                        CalcTimeUsed();
                        MakeNewTimeStamp();
                        this.DeviceID = "";
                    }
                }
            }
        }

        public void serialportstatuschanged_EventHandler(object sender, EventArrivedEventArgs e)
        {
            MessageBox.Show("Change detected.");
        }

    }
}
//USB\VID_13B1&PID_0039\000000000001