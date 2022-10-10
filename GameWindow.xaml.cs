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
        int dropSpeed = 10;
        bool goLeft, goRight;

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
            // when detection works this can be used to drop the player on the platforms
            // Canvas.SetTop(Player, Canvas.GetTop(Player) + dropSpeed);

            if (goLeft == true && Canvas.GetLeft(Player) > 0)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
            }
            if (goRight == true && Canvas.GetLeft(Player) + (Player.Width + 15) < Application.Current.MainWindow.Width)
            {
                Canvas.SetLeft(Player, Canvas.GetLeft(Player) + speed);
            }

            // check if player leaves screen.
            // TODO: Fix error no height somehow
            //if (Canvas.GetTop(Player) + (Player.Height * 2) > Application.Current.MainWindow.Height)
            //{
            //    Canvas.SetTop(Player, -200);
            //}

            //// colission detection
            //foreach (var x in gameCanvas.Children.OfType<Rectangle>())
            //{
            //    if ((string)x.Tag == "Platform")
            //    {
            //        x.Stroke = Brushes.Black;
            //        Rect playerHitBox = new Rect(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);
            //        Rect platformHitBox = new Rect(Canvas.GetLeft(x), Canvas.GetTop(x), x.Width, x.Height);

            //        if (playerHitBox.IntersectsWith(platformHitBox))
            //        {
            //            dropSpeed = 0;
            //            Canvas.SetTop(Player, Canvas.GetTop(x) - Player.Height);
            //        }
            //        else
            //        {
            //            dropSpeed = 10;
            //        }
            //    }
            //}
        }

        private void KeyIsDown(object sender, KeyEventArgs e)
        {
            // When key is down a will be true for player movement
            bool a = true;
            MovePlayer(e, a);
        }

        private void KeyIsUp(object sender, KeyEventArgs e)
        {
            // When key is up a will be false for player movement
            bool a = false;
            MovePlayer(e, a);
        }

        /// <summary>
        /// Bepaald of speler naar rechts gaat of naar links
        /// </summary>
        /// <param name="e">Welke key is ingedrukt</param>
        /// <param name="a">true of false afhankelijk van key up of key down</param>
        private void MovePlayer(KeyEventArgs e, bool a)
        {
            if (e.Key == Key.Left)
            {
                goLeft = a;
            }
            if (e.Key == Key.Right)
            {
                goRight = a;
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
