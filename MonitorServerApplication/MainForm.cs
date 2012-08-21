using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MonitorServerApplication
{
    public partial class MainServerForm : Form
    {
        ServerLog curentLog;
        ServerMainThread serverThread;
        public MainServerForm()
        {
            InitializeComponent();
        }

        private void bStart_Click(object sender, EventArgs e)
        {
            curentLog = new ServerListLog();
            serverThread = new ServerMainThread(5555);
        }
    }
}
