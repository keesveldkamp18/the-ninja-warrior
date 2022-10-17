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

namespace project_arcade
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void one_player_button(object sender, RoutedEventArgs e)
        {
            _1spelerkeuze GW = new _1spelerkeuze();
            GW.Visibility = Visibility.Visible;
            this.Close();
        }

        private void two_player_button(object sender, RoutedEventArgs e)
        {
            _2spelerkeuze gw = new _2spelerkeuze();
            gw.Visibility = Visibility.Visible;
            this.Close();
        }

        private void Highscores_button(object sender, RoutedEventArgs e)
        {
            HighscoresWindow GW = new HighscoresWindow();
            GW.Visibility = Visibility.Visible;
            this.Close();
        }

        private void Quit_button(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

    }
}
