using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;
using MonitorServerApplication.ServerThreading;

namespace MonitorServerApplication
{
    public partial class MainServerForm : Form
    {
        private ServerLog _curentLog;
        private ServerMainThread _serverThread;

        delegate void SetItem(LogItem item);

        private BackgroundWorker _backgroundWorker;

        private readonly ConcurrentQueue<LogItem> _logItems;

        public MainServerForm()
        {
            InitializeComponent();
            _backgroundWorker = new System.ComponentModel.BackgroundWorker();
            _backgroundWorker.RunWorkerCompleted += this.BackgroundWorkerDoWork;
            _logItems = new ConcurrentQueue<LogItem>();
        }

        private void SetItemInvoke (LogItem item)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (this.LogView.InvokeRequired)
            {
                var d = new SetItem(SetItemInvoke);
                this.Invoke(d, new object[] { item });
            }
            else
            {
                var logitem = LogView.Items.Add(item.Time.ToString());
                logitem.SubItems.Add(item.IP);
                logitem.SubItems.Add(item.Message);
            }
        }

        private void BackgroundWorkerDoWork(object sender, 
			RunWorkerCompletedEventArgs e)
        {
            LogItem item;
            while (_logItems.TryDequeue(out item))
            {
 
            }
                
        }

        void NewLogEvent(Object sender, LogItemEventArgs e)
        {
            SetItemInvoke(e.item);
        }

        private void BStartClick(object sender, EventArgs e)
        {
            _curentLog = new ServerListLog();
            _serverThread = new ServerMainThread(5555);
            _serverThread.StartClients();
            _serverThread.StartWriter();
            _serverThread._DBWriter.LogItemSaveEvent += NewLogEvent;
            bStart.Enabled = false;
            bStop.Enabled = true;
        }

        private void BStopClick(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (_serverThread == null) 
                 return;

            _serverThread.Stop();
            _serverThread = null;
            bStart.Enabled = true;
            bStop.Enabled = false;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataDecoder.DoTestChiper();
        }

        private void LogTimerTick(object sender, EventArgs e)
        {
           /* if (_serverThread != null)
                if (_serverThread._DBWriter.LogItems != null)
                {
                    //No cash - no hash
                    if (_serverThread._DBWriter.LogItems.Count == 0)
                        return;

                    LogView.BeginUpdate();
                    try
                    {
                        LogItem item;
                        while (_serverThread._DBWriter.LogItems.TryDequeue(out item))
                        {
                            var logitem = LogView.Items.Add(item.Time.ToString());
                            logitem.SubItems.Add(item.IP);
                            logitem.SubItems.Add(item.Message);
                        }

                    }
                    finally
                    {
                        LogView.EndUpdate();
                        LogView.AutoResizeColumn(2, ColumnHeaderAutoResizeStyle.ColumnContent);
                    }
                }*/
        }

        private void MainServerForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            StopServer();
        }
    }
}
