using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace ALARMS_x86
{
    public partial class UserDetails : Form
    {
        private DeviceManager devMgr;

        public UserDetails()
        {
            InitializeComponent();
            this.devMgr = DeviceManager.GetDeviceMgr();
            this.Focus();
            textBox1.Focus();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(textBox1.Text))
            {
                devMgr.UserNameEntered(textBox1.Text);
                this.Close();
            }
        }
    }
}
