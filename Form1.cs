using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Data.SqlClient;
using System.Configuration;
using System.Management;
using System.Data.SQLite;

namespace ALARMS_x86
{
    public partial class Form1 : Form
    {
        public Boolean timerstarted = false;
        private DeviceManager devMgr;
        private NotifyIcon mynotifyicon;

        public Form1()
        {
            InitializeComponent();
            this.devMgr = DeviceManager.GetDeviceMgr();
            this.mynotifyicon = new NotifyIcon();
            mynotifyicon.DoubleClick += this.mynotifyicon_DoubleClicked;
            getDevReg();
        }

        private void setInstrumentGUIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetGUID setGUID = new SetGUID();
            setGUID.ShowDialog();
        }

        private void exportToolStripMenuItem_Click(object sender, EventArgs e)
        {
            LogExporter exporter = new LogExporter();
            exporter.Show();
        }

        private void getDevReg()
        {
            comboBox1.Items.Clear();
            foreach (string item in devMgr.DeviceNameToMon)
            {
                comboBox1.Items.Add(item);
            }
        }

        private void PrepareDataGrid(string devicename)
        {
            QueryManager qMgr = new QueryManager();
            dataGridView1.DataSource = qMgr.RetrieveData("TimeLog", "DeviceName", "'" + comboBox1.SelectedItem.ToString() + "'");
        }

        private void comboBox1_SelectionChangeCommitted(object sender, EventArgs e)
        {
            PrepareDataGrid(comboBox1.SelectedItem.ToString());
        }

        private void comboBox1_DropDown(object sender, EventArgs e)
        {
            getDevReg();
        }

        protected virtual void mynotifyicon_DoubleClicked(object source, EventArgs e)
        {
            this.Show();
            this.WindowState = FormWindowState.Normal;
        }

        private void exitToolStripMenuItem_Click_1(object sender, EventArgs e)
        {
            Application.Exit();
        }

        private void deleteTimeLogsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryManager qMgr = new QueryManager();
            qMgr.DeleteAll("TimeLog");
        }

        private void deleteDeviceToolStripMenuItem_Click(object sender, EventArgs e)
        {
            QueryManager qMgr = new QueryManager();
            qMgr.DeleteAll("Instruments");
            getDevReg();
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {

        }
    }
}
