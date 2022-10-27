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
    /// Interaction logic for _1spelerkeuze.xaml
    /// </summary>
    public partial class _1spelerkeuze : Window
    {
        public _1spelerkeuze()
        {
            InitializeComponent();
        }

        private void OnePlayerToMain(object sender, RoutedEventArgs e)
        {
            MainWindow gw = new MainWindow();
            gw.Visibility = Visibility.Visible;
            this.Close();
        }

        private void OnePlayerToGame(object sender, RoutedEventArgs e)
        {
            GameWindow gw = new GameWindow(false);
            gw.Visibility = Visibility.Visible;
            this.Close();
        }
    }
}
