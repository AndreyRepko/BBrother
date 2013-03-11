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
    public partial class MainWindow : Window
    {
        private Boolean IsConnected = false;
        private Npgsql.NpgsqlConnection PG;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void ConnectButtonClick(object sender, RoutedEventArgs e)
        {
            if (!IsConnected)
            {
                var pg_con = new Npgsql.NpgsqlConnectionStringBuilder();
                pg_con.Host = "127.0.0.1";
                pg_con.UserName = "bbrother_admin";
                pg_con.Password = "qwerty";
                pg_con.Database = "bbrother";
                PG = new Npgsql.NpgsqlConnection(pg_con.ConnectionString);
                PG.Open();
                IsConnected = true;
                ConnectButton.Content = "Diconnect";
            }
            else
            {
                IsConnected = false;
                PG.Close();
                ConnectButton.Content = "Connect to DB...";
            }
        }

        private void RefreshButtonClick(object sender, RoutedEventArgs e)
        {
            var pgQuery = new Npgsql.NpgsqlCommand("SELECT * FROM info_log");
            pgQuery.Connection = PG;
            var reader = pgQuery.ExecuteReader();
            ServerLogGrid.ItemsSource = reader;
            ServerLogGrid.Items.Refresh();
        }
    }
}
