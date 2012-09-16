using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonitorServerApplication.Packets;
using MonitorServerApplication.ServerThreading;

namespace MonitorServerApplication
{
    public partial class MainServerForm : Form
    {
        ServerLog _curentLog;
        ServerMainThread _serverThread;
        public MainServerForm()
        {
            InitializeComponent();
        }

        private void BStartClick(object sender, EventArgs e)
        {
            _curentLog = new ServerListLog();
            _serverThread = new ServerMainThread(5555);
            _serverThread.Start();
            bStart.Enabled = false;
            bStop.Enabled = true;
        }

        private void BStopClick(object sender, EventArgs e)
        {
            _serverThread.Stop();
            _serverThread = null;
            bStart.Enabled = true;
            bStop.Enabled = false;

        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataDecoder.DoTestChiper();
        }
    }
}
