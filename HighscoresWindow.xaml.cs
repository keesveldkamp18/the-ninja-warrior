using MySql.Data.MySqlClient;
using System;
using System.Data;
using System.Windows;
using System.Windows.Controls;

namespace project_arcade
{
    /// <summary>
    /// Interaction logic for HighscoresWindow.xaml
    /// </summary>
    public partial class HighscoresWindow : Window
    {

        //might need to download and reference the connector. Download: https://dev.mysql.com/downloads/connector/net/ 
        string connectionString = "SERVER=web0113.zxcs.nl,3306; DATABASE=u45926p46412_highscore; UID=u45926p46412_highscore; PASSWORD=ninjaEndlessRunner;";

        public HighscoresWindow()
        {
            InitializeComponent();

            //Connect to DB
            MySqlConnection connection = new MySqlConnection(connectionString);

            //open connection
            connection.Open();

            //Retreive scores from DB
            MySqlCommand cmd = new MySqlCommand("SELECT * FROM scores ORDER BY playerScore DESC LIMIT 20", connection);

            DataTable dt = new DataTable();
            dt.Load(cmd.ExecuteReader());
            connection.Close();

            dt.Columns.Add("rank", typeof(String));

            foreach(DataRow row in dt.Rows)
            {
                row.SetField("rank", dt.Rows.IndexOf(row) + 1 + ".");
            }

            highscoreList.DataContext = dt;
        }
        //TODO: database legen toevoegen
        private void Leeg_Database(object sender, RoutedEventArgs e)
        {

        }

        private void Terug_Naar_Startscherm(object sender, RoutedEventArgs e)
        {
            MainWindow gw = new MainWindow();
            gw.Visibility = Visibility.Visible;
            this.Close();
        }

        private void highscoreList_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
