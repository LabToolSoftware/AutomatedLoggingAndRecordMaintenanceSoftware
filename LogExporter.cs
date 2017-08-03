using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace ALARMS_x86
{
    public partial class LogExporter : Form
    {
        private QueryManager qMgr;
        private Dictionary<String, LogicalDevice> LDevDict;
        private Dictionary<String, TimeLog> LogDict;
        private string filename;
        private string filedestination;

        public LogExporter()
        {
            InitializeComponent();
            qMgr = new QueryManager();
            GetDevices();
        }

        private void GetDevices()
        {
            DataTable LDevDict = qMgr.RetrieveData("Instruments");
            foreach (DataRow row in LDevDict.Rows)
            {
                comboBox1.Items.Add(row["DeviceName"].ToString());
            }
        }
        private void GetLog()
        {
            if (filedestination != null || filedestination.Length > 0)
            {
                QueryManager qMgr = new QueryManager();
                DataTable LogDict = qMgr.RetrieveData("TimeLog", "DeviceName", "'" + comboBox1.SelectedItem.ToString() + "'");

                using (StreamWriter file = new StreamWriter(filedestination))
                {
                    string header = "User,Date Used, Time On, Time Off, Time Used";
                    file.WriteLine("Log for: " + comboBox1.SelectedItem.ToString());
                    file.WriteLine(header);
                    foreach (DataRow log in LogDict.Rows)
                    {
                        string[] propertylist = new string[5];
                        propertylist[0] = log["UserName"].ToString();
                        propertylist[1] = log["DateUsed"].ToString();
                        propertylist[2] = log["TimeOn"].ToString();
                        propertylist[3] = log["TimeOff"].ToString(); ;
                        propertylist[4] = log["TimeUsed"].ToString();
                        string line = String.Join(",", propertylist);
                        file.WriteLine(line);
                    }
                }
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (comboBox1.SelectedItem != null)
            {
                GetLog();
                MessageBox.Show("Log exported for: " + comboBox1.SelectedItem.ToString());
            }
        }

        private void button2_Click(object sender, EventArgs e)
        {
            SaveFileDialog sf = new SaveFileDialog();
            sf.FileName = filename;
            sf.Filter = "csv files (*.csv)|*.csv";
            if (sf.ShowDialog() == DialogResult.OK)
            {
                filedestination = Path.GetFullPath(sf.FileName);
            }
            richTextBox1.Text = filedestination;
        }
    }
}
