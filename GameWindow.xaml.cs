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
            timer.Tick += GameEngine;
            timer.Interval = TimeSpan.FromMilliseconds(20);
            timer.Start();
        }

        private void GameEngine(object? sender, EventArgs e)
        {
            #region player movement
            // move left
            if (Keyboard.IsKeyDown(Key.Left) && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
            }
            // move right
            if (Keyboard.IsKeyDown(Key.Right))
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + speed);
            }
            //jump
            if(Keyboard.IsKeyDown(Key.Space))
            {
                gravity = -20;
            }
            // gravity use gravity to fall back down.       
            Canvas.SetTop(Player, Canvas.GetTop(Player) + gravity);
            gravity++;
            #endregion
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
