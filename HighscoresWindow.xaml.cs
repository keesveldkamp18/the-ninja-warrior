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
using System.Windows.Shapes;

namespace project_arcade
{
    /// <summary>
    /// Interaction logic for HighscoresWindow.xaml
    /// </summary>
    public partial class HighscoresWindow : Window
    {
        public HighscoresWindow()
        {
            InitializeComponent();
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
    }
}
