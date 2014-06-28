using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;
using MonitorServerApplication.ServerThreading;

namespace MonitorServerApplication
{
    public partial class MainServerForm : Form
    {
        private Task _DBDataTask;
      //  private ServerLog _curentLog;
        delegate void SetItem(LogItem item);

        //Global Token for cancelation 
        private CancellationTokenSource cts;

        private readonly ConcurrentQueue<LogItem> _logItems;

        public MainServerForm()
        {
            InitializeComponent();
            _logItems = new ConcurrentQueue<LogItem>();
        }

        private void SetItemInvoke (LogItem item)
        {
            // InvokeRequired required compares the thread ID of the
            // calling thread to the thread ID of the creating thread.
            // If these threads are different, it returns true.
            if (LogView.InvokeRequired)
            {
                Invoke(new SetItem(SetItemInvoke), 
                       new object[] { item });
            }
            else
            {
                var logitem = LogView.Items.Add(item.Time.ToString(CultureInfo.InvariantCulture));
                logitem.SubItems.Add(item.IP);
                logitem.SubItems.Add(item.Message);
            }
        }

        private void OutputLogToGrid(CancellationToken ct)
        {
            while (!ct.IsCancellationRequested)
            {
                LogItem item;
                while (_logItems.TryDequeue(out item))
                {
                    SetItemInvoke(item);
                }
                //We don't like to have  
                Thread.Sleep(1);
            }

        }

        void NewLogEvent(Object sender, LogItemEventArgs e)
        {
            SetItemInvoke(e.item);
        }

        private void BStartClick(object sender, EventArgs e)
        {
            cts = new CancellationTokenSource();

            var DBData = new ServerWriter();
            DBData.LogItemSaveEvent += NewLogEvent;
            (new Task(() => ServerMainThread.DoAcceptConnectionsAsync(5555, DBData, DBData, cts.Token), cts.Token)).Start();
            bStart.Enabled = false;
            bStop.Enabled = true;

            _DBDataTask = new Task(() => DBData.Save(cts.Token), cts.Token);
            _DBDataTask.Start();

            //Create write to the grid task (it will be dead as cts is fired
            (new Task(() => OutputLogToGrid(cts.Token), cts.Token)).Start();

        }

        private void BStopClick(object sender, EventArgs e)
        {
            StopServer();
        }

        private void StopServer()
        {
            if (cts != null)
            {
                cts.Cancel();
                cts = null;
            }

            if (_DBDataTask != null)
            {
                //Here can be a lot of exceptions thrown, but would like to see them all   
                if (!_DBDataTask.Wait(10000))
                    throw new Exception("Can't stop DB Writer Task!");
                _DBDataTask = null;
            }
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
