using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BBFrontend
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public sealed partial class MainWindow : Window, IDisposable
    {
        private Boolean _isConnected = false;
        private Npgsql.NpgsqlConnection _pgConnection;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (!_isConnected)
            {
                var pg_con = new Npgsql.NpgsqlConnectionStringBuilder();
                pg_con.Host = "127.0.0.1";
                pg_con.UserName = "bbrother_admin";
                pg_con.Password = "qwerty";
                pg_con.Database = "bbrother";
                _pgConnection = new Npgsql.NpgsqlConnection(pg_con.ConnectionString);
                _pgConnection.Open();
                _isConnected = true;
                ConnectButton.Content = "Diconnect";
            }
            else
            {
                _isConnected = false;
                _pgConnection.Close();
                ConnectButton.Content = "Connect to DB...";
            }
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            var pgQuery = new Npgsql.NpgsqlCommand("SELECT * FROM info_log");
            pgQuery.Connection = _pgConnection;
            var reader = pgQuery.ExecuteReader();
            ServerLogGrid.ItemsSource = reader;
            ServerLogGrid.Items.Refresh();
        }

        public void Dispose()
        {
            if (_pgConnection!=null)
                _pgConnection.Dispose();
        }
    }
}
