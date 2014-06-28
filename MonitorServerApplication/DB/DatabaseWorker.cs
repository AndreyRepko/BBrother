using System;
using MonitorServerApplication.Loging;
using Npgsql;
using NpgsqlTypes;

namespace MonitorServerApplication.DB
{
    class DatabaseWorker
    {
        private NpgsqlCommand _insertEventLogItemCommand;
        private NpgsqlCommand _getSettingsCommand;
        private readonly NpgsqlConnection _pgConnect;
        public bool IsConnected = false;

        public DatabaseWorker()
        {
            var connectionParams = new NpgsqlConnectionStringBuilder();
            connectionParams.Host = "127.0.0.1";
            connectionParams.UserName = "bbAdmin";
            connectionParams.Password = "qwerty";
            connectionParams.Database = "BigBrotherDB";
            _pgConnect = new NpgsqlConnection(connectionParams.ConnectionString);
            _pgConnect.Open();
        }

        public DatabaseWorker(NpgsqlConnectionStringBuilder connectionParams)
        {
            _pgConnect = new NpgsqlConnection(connectionParams.ConnectionString);
        }

        public DatabaseWorker(NpgsqlConnection connection)
        {
            _pgConnect = connection;
        }

        public void SaveItem(LogItem item)
        {
           if (_insertEventLogItemCommand == null)
           {
             _insertEventLogItemCommand =  new NpgsqlCommand("INSERT INTO event_log (event_time, ip, user_name, event, code) VALUES (:event_time, :ip, :user_name, :event, :code)");
             _insertEventLogItemCommand.Parameters.Add("event_time", NpgsqlDbType.TimestampTZ); // 0
             _insertEventLogItemCommand.Parameters.Add("ip", NpgsqlDbType.Text);                // 1
             _insertEventLogItemCommand.Parameters.Add("user_name", NpgsqlDbType.Text);         // 2 
             _insertEventLogItemCommand.Parameters.Add("event", NpgsqlDbType.Text);             // 3
             _insertEventLogItemCommand.Parameters.Add("code", NpgsqlDbType.Integer);           // 4
             _insertEventLogItemCommand.Connection = _pgConnect;
           }
                // Now, add a value to it and later execute the command as usual.
           _insertEventLogItemCommand.Parameters[0].Value = item.Time;
           _insertEventLogItemCommand.Parameters[1].Value = item.IP;
           _insertEventLogItemCommand.Parameters[1].Size = item.IP.Length;
           _insertEventLogItemCommand.Parameters[3].Value = item.Message;
           _insertEventLogItemCommand.Parameters[3].Size = item.Message.Length;
           _insertEventLogItemCommand.ExecuteNonQuery();
        }

        public ClientSettings GetSettings(SettingsType settingsType)
        {
            //TODO: Rewrite it to get from DB
           return new ClientSettings(); 
        }

    }
}
