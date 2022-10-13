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

using System.Windows.Threading;

namespace project_arcade
{
    /// <summary>
    /// Interaction logic for GameWindow.xaml
    /// </summary>
	public partial class GameWindow : Window
    {

        DispatcherTimer timer = new DispatcherTimer();

        int speed = 10;
        int gravity = 0;

        public GameWindow()
        {
            InitializeComponent();

            gameCanvas.Focus();
            timer.Tick += MainTimerEvent;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private void MainTimerEvent(object? sender, EventArgs e)
        {
            #region player gravity
            // makes the player jump and use gravity to fall back down.       
            Canvas.SetTop(Player, Canvas.GetTop(Player) + gravity);
            gravity++;
            #endregion
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            MovePlayer(e);
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            MovePlayer(e);
        }

        /// <summary>
        /// Decides the player movement.
        /// </summary>
        /// <param name="e">Wich key is pressed</param>
        private void MovePlayer(KeyEventArgs e)
        {
            if (e.Key == Key.Left && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
            }
            if (e.Key == Key.Right)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + speed);
            }
            if (e.Key == Key.Space)
            {
                // lower the number the heigher it will jump
                gravity = -20;
            }
        }

        /// <summary>
        /// Return to menu
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ReturnToMenuBtn(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }
    }
}
