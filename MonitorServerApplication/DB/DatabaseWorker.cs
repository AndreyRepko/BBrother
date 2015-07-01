using System;
using MonitorServerApplication.Loging;
using MonitorServerApplication.Packets;
using Npgsql;
using NpgsqlTypes;

namespace MonitorServerApplication.DB
{
    class DatabaseWorker
    {
        private NpgsqlCommand _insertEventLogItemCommand;
        private NpgsqlCommand _insertInfoMessageItemCommand;

        private NpgsqlCommand _getSettingsCommand;
        private readonly NpgsqlConnection _pgConnect;
        public bool IsConnected = false;

        public DatabaseWorker()
        {
            var connectionParams = new NpgsqlConnectionStringBuilder();
            connectionParams.Host = "127.0.0.1";
            connectionParams.UserName = "bbadmin";
            connectionParams.Password = "qwerty";
            connectionParams.Database = "bbrother";
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
             _insertEventLogItemCommand =  new NpgsqlCommand("INSERT INTO event_log (event_time, ip, user_name, event) VALUES (:event_time, :ip, :user_name, :event)");
             _insertEventLogItemCommand.Parameters.Add("event_time", NpgsqlDbType.TimestampTZ); // 0
             _insertEventLogItemCommand.Parameters.Add("ip", NpgsqlDbType.Text);                // 1
             _insertEventLogItemCommand.Parameters.Add("user_name", NpgsqlDbType.Text);         // 2 
             _insertEventLogItemCommand.Parameters.Add("event", NpgsqlDbType.Text);             // 3
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

        public void SaveItem(InfoMessage item)
        {
            //Todo: handle last application better
            if (item.MessageType == -10)
                return;

            if (_insertInfoMessageItemCommand == null)
            {
                _insertInfoMessageItemCommand = new NpgsqlCommand("INSERT INTO log (event_id, description, client_time,route_type, ip, user_name) VALUES " +
                                                                  "(:event_id, :description, :client_time, :route_type, :ip, :user_name)");
                _insertInfoMessageItemCommand.Parameters.Add("event_id", NpgsqlDbType.Integer); // 0
                _insertInfoMessageItemCommand.Parameters.Add("description", NpgsqlDbType.Text);                // 1
                _insertInfoMessageItemCommand.Parameters.Add("client_time", NpgsqlDbType.Timestamp);         // 2 
                _insertInfoMessageItemCommand.Parameters.Add("route_type", NpgsqlDbType.Smallint);             // 3
                _insertInfoMessageItemCommand.Parameters.Add("ip", NpgsqlDbType.Text);           // 4
                _insertInfoMessageItemCommand.Parameters.Add("user_name", NpgsqlDbType.Text);           // 5
                _insertInfoMessageItemCommand.Connection = _pgConnect;
            }
            // Now, add a value to it and later execute the command as usual.
            _insertInfoMessageItemCommand.Parameters[0].Value = item.MessageType;
            _insertInfoMessageItemCommand.Parameters[1].Value = item.Info;
            _insertInfoMessageItemCommand.Parameters[1].Size = item.Info.Length;
            _insertInfoMessageItemCommand.Parameters[2].Value = item.Time;
            _insertInfoMessageItemCommand.Parameters[3].Value = -1;
            _insertInfoMessageItemCommand.Parameters[4].Value = item.IP;
            _insertInfoMessageItemCommand.Parameters[4].Size = item.IP.Length;
            _insertInfoMessageItemCommand.Parameters[5].Value = item.UserName;
            _insertInfoMessageItemCommand.Parameters[5].Size = item.UserName.Length;
            _insertInfoMessageItemCommand.ExecuteNonQuery();
        }

        public ClientSettings GetSettings(SettingsType settingsType)
        {
            //TODO: Rewrite it to get from DB
           return new ClientSettings(); 
        }

    }
}
