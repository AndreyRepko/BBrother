using System;
using MonitorServerApplication.Loging;
using Npgsql;
using NpgsqlTypes;

namespace MonitorServerApplication.DB
{
    class DatabaseWriter
    {
        private NpgsqlCommand _infoLogCommand;
        private readonly NpgsqlConnection _pgConnect;
        public bool IsConnected = false;

        public DatabaseWriter()
        {
            var connectionParams = new NpgsqlConnectionStringBuilder();
            connectionParams.Host = "127.0.0.1";
            connectionParams.UserName = "bbrother_admin";
            connectionParams.Password = "qwerty";
            connectionParams.Database = "bbrother";
            _pgConnect = new NpgsqlConnection(connectionParams.ConnectionString);
            _pgConnect.Open();
        }

        public DatabaseWriter(NpgsqlConnectionStringBuilder connectionParams)
        {
            _pgConnect = new NpgsqlConnection(connectionParams.ConnectionString);
        }

        public DatabaseWriter(NpgsqlConnection connection)
        {
            _pgConnect = connection;
        }

        public void SaveItem(LogItem item)
        {
           if (_infoLogCommand == null)
           {
             _infoLogCommand =  new NpgsqlCommand("INSERT INTO info_log (event_time, ip, user_name, event, code) VALUES (:event_time, :ip, :user_name, :event, :code)");
             _infoLogCommand.Parameters.Add("event_time", NpgsqlDbType.TimestampTZ); // 0
             _infoLogCommand.Parameters.Add("ip", NpgsqlDbType.Text);                // 1
             _infoLogCommand.Parameters.Add("user_name", NpgsqlDbType.Text);         // 2 
             _infoLogCommand.Parameters.Add("event", NpgsqlDbType.Text);             // 3
             _infoLogCommand.Parameters.Add("code", NpgsqlDbType.Integer);           // 4
             _infoLogCommand.Connection = _pgConnect;
           }
                // Now, add a value to it and later execute the command as usual.
           _infoLogCommand.Parameters[0].Value = item.Time;
           _infoLogCommand.Parameters[1].Value = item.IP;
           _infoLogCommand.Parameters[1].Size = item.IP.Length;
           _infoLogCommand.Parameters[3].Value = item.Message;
           _infoLogCommand.Parameters[3].Size = item.Message.Length;
           _infoLogCommand.ExecuteNonQuery();
        }


    }
}
