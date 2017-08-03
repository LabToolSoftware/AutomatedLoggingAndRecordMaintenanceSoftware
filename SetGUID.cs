using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO.Ports;

namespace ALARMS_x86
{
    public partial class SetGUID : Form
    {
        private DeviceManager devMgr;
        private Form1 mainform;

        private delegate void NewDeviceAddedToRegistryEventHandler(object source, EventArgs e);
        private event NewDeviceAddedToRegistryEventHandler NewDeviceAddedToRegistry;

        public SetGUID()
        {
            InitializeComponent();
            devMgr = DeviceManager.GetDeviceMgr();

            this.NewDeviceAddedToRegistry += devMgr.OnNewDevAddedToRegEventHandler;



        }

        private void PopulateDevList()
        {
            comboBox1.Items.Clear();
            devMgr.GetSysDevList();
            foreach (var item in devMgr.GetSysDevList())
            {
                comboBox1.Items.Add(item);
            }
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                LogicalDevice new_Dev = new LogicalDevice();
                new_Dev.DeviceID = comboBox1.SelectedItem.ToString();
                new_Dev.Devicename = textBox2.Text.Replace(' ', '_');
                new_Dev.dateoflastservice = DateTime.Now.ToShortDateString();
                new_Dev.timeused = "0";

                QueryManager qMgr = new QueryManager();

                qMgr.InsertRow("Instruments", new_Dev);
                MessageBox.Show("Device added:" + new_Dev.Devicename);
                OnNewDevAddedToReg();
            }
            catch (Exception)
            {
                MessageBox.Show("Please Select a Device From the List");
            }
        }

        protected virtual void OnNewDevAddedToReg()
        {
            if (NewDeviceAddedToRegistry != null)
            {
                NewDeviceAddedToRegistry(this, EventArgs.Empty);
            }
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            MessageBox.Show("Disconnect the device of interest. Then press OK.");
            devMgr.GetSysDevList();
            List<string> old_list = devMgr.GetSysDevList();
            MessageBox.Show("Please reconnect device and ensure it is on.Then Press OK.");
            devMgr.GetSysDevList();
            List<string> new_list = devMgr.GetSysDevList();
            var newDeviceID = new_list.Except(old_list);

            if (newDeviceID.Count() != 0)
            {
                comboBox1.Items.Clear();
                foreach (var item in newDeviceID)
                {
                    comboBox1.Items.Add(item);
                }
            }

        }

        private void GetComPorts()
        {
            comboBox1.Items.Clear();
            var ArrayPortNames = SerialPort.GetPortNames();
            foreach (var portname in ArrayPortNames)
            {
                comboBox1.Items.Add(portname);
            }
        }

        private void radioButton2_CheckedChanged(object sender, EventArgs e)
        {
            PopulateDevList();
            label1.Text = "Select Device ID:";
        }

        private void radioButton1_CheckedChanged(object sender, EventArgs e)
        {
            GetComPorts();
            label1.Text = "Select COM Port:";
        }

        private void button2_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
