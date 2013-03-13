using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Npgsql;

namespace MonitorServerApplication.ServerThreading
{
    class DatabaseWriter
    {
        public NpgsqlConnection PgConnect;
        public bool IsConnected = false;

        public DatabaseWriter(NpgsqlConnectionStringBuilder connectionParams)
        {
            PgConnect = new NpgsqlConnection(connectionParams.ConnectionString);
        }

        protected void CheckConnected()
        {
           
            /*if (PgConnect.Te)
        {
        }*/

        }

    }
}
