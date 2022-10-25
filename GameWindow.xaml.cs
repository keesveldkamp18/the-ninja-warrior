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
		double lastPlayerTop;
		bool onFloor;

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
			#region player move controls
			// Horizontal movement
			if(Keyboard.IsKeyDown(Key.Left))
			{
				Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
			}
			if(Keyboard.IsKeyDown(Key.Right))
			{
				Canvas.SetLeft(Player, Canvas.GetLeft(Player) + speed);
			}

			// Jumping
			if(Keyboard.IsKeyDown(Key.Space) && onFloor)
			{
				gravity = -20;
				onFloor = false;
			}
			#endregion

			#region Player Gravity
			// Makes the player fall down
			lastPlayerTop = Canvas.GetTop(Player);
			gravity++;
			Canvas.SetTop(Player, Canvas.GetTop(Player) + gravity);
			#endregion

			#region player collision detection
			Rect playerRect = new(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);

			foreach(var rectangle in gameCanvas.Children.OfType<Rectangle>())
			{
				if((string)rectangle.Tag == "Platform")
				{
					Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

					// Don't let the player fall through the platform iff the player was above or on the platform the previous tick
					if(playerRect.IntersectsWith(platformRect) && lastPlayerTop + Player.Height <= Canvas.GetTop(rectangle))
					{
						gravity = 0;
						Canvas.SetTop(Player, Canvas.GetTop(rectangle) - Player.Height);
						onFloor = true;
					}
				}
			}
			#endregion

			#region player screen bounds detection
			// if the player goes too far left or right it puts them back
			if(Canvas.GetLeft(Player) < 0)
			{
				Canvas.SetLeft(Player, 0);
			}
			else if(Canvas.GetLeft(Player) > 1525)
			{
				Canvas.SetLeft(Player, 1525);
			}
            #endregion

			//ff testen weet niet wat ik doe. dit is voor de naam van de speler

            // Horizontal movement of the player name
            if (Keyboard.IsKeyDown(Key.Left))
            {
                Canvas.SetLeft(speler1naam, Canvas.GetLeft(speler1naam) - speed);
            }
            if (Keyboard.IsKeyDown(Key.Right))
            {
                Canvas.SetLeft(speler1naam, Canvas.GetLeft(speler1naam) + speed);
            }

            // Jumping for the playername
            if (Keyboard.IsKeyDown(Key.Space) && onFloor)
            {
                gravity = -20;
                onFloor = false;
            }

            if (Canvas.GetLeft(speler1naam) < 0)
            {
                Canvas.SetLeft(speler1naam, 0);
            }
            else if (Canvas.GetLeft(speler1naam) > 1525)
            {
                Canvas.SetLeft(speler1naam, 1525);
            }

            // Makes the player name fall down

            double LastPlayernameTop;

            LastPlayernameTop = Canvas.GetTop(speler1naam);
            gravity++;
            Canvas.SetTop(speler1naam, Canvas.GetTop(speler1naam) + gravity);

			//fills the rectangle with the player name
			ImageBrush spelernaam1 = new ImageBrush();
			spelernaam1.ImageSource = new BitmapImage(new Uri("C:\\school\\jaar1.2\\periode_1\\programmeren\\project-arcade\\images\\speler1.png"));
			speler1naam.Fill = spelernaam1;

            //TODO; collision with player instead of the platform
            //makes it so the playername stays above the player
            Rect playernamerect = new(Canvas.GetLeft(speler1naam), Canvas.GetTop(speler1naam), speler1naam.Width, speler1naam.Height);

            foreach (var rectangle in gameCanvas.Children.OfType<Rectangle>())
            {
                if ((string)rectangle.Tag == "Platform")
                {
                    Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

                    // Don't let the player fall through the platform iff the player was above or on the platform the previous tick
                    if (playernamerect.IntersectsWith(platformRect) && lastPlayerTop + speler1naam.Height <= Canvas.GetTop(rectangle))
                    {
                        gravity = 0;
                        Canvas.SetTop(speler1naam, Canvas.GetTop(rectangle) - speler1naam.Height);
                        onFloor = true;
                    }
                }
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
