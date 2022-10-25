using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace project_arcade
{
	public partial class GameWindow : Window
	{
		#region Variables
		DispatcherTimer timer = new DispatcherTimer();

		int speed = 10;
		int gravity;
		double lastPlayerTop;
		bool onFloor;
		#endregion

		public GameWindow()
		{
			InitializeComponent();

			InitializeGameWindow();
		}

		private void InitializeGameWindow()
		{
			timer.Tick += GameEngine;
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Start();
		}

		private void GameEngine(object? sender, EventArgs e)
		{
			PlayerMovement();

			PlayerGravity();

			PlayerCollisionDetection();

			PlayerScreenBoundsDetection();

			PauseChecking();

			BackgroundParallax();
		}

		private void PlayerScreenBoundsDetection()
		{
			// Keep the player within view
			if(Canvas.GetLeft(Player) < 0)
			{
				Canvas.SetLeft(Player, 0);
			}
			else if(Canvas.GetLeft(Player) > 1525)
			{
				Canvas.SetLeft(Player, 1525);
			}
		}

		private void PlayerMovement()
		{
			// Move left if the left arrow key is held and/or right if the right arrow key is held
			if(Keyboard.IsKeyDown(Key.Left))
			{
				Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
			}

			if(Keyboard.IsKeyDown(Key.Right))
			{
				Canvas.SetLeft(Player, Canvas.GetLeft(Player) + speed);
			}

			// Adds negative force if the spacebar is pressed
			if(Keyboard.IsKeyDown(Key.Space) && onFloor)
			{
				gravity = -20;
				onFloor = false;
			}
		}

		private void PlayerGravity()
		{
			// Makes the player fall down
			lastPlayerTop = Canvas.GetTop(Player);
			gravity++;
			Canvas.SetTop(Player, Canvas.GetTop(Player) + gravity);
		}

		private void PlayerCollisionDetection()
		{
			Rect playerRect = new(Canvas.GetLeft(Player), Canvas.GetTop(Player), Player.Width, Player.Height);

			foreach(var rectangle in gameCanvas.Children.OfType<Rectangle>())
			{
				if((string)rectangle.Tag == "Platform")
				{
					Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

					// Don't let the player fall through a platform if the player was above or on it the previous tick
					if(playerRect.IntersectsWith(platformRect) && lastPlayerTop + Player.Height <= Canvas.GetTop(rectangle))
					{
						gravity = 0;
						Canvas.SetTop(Player, Canvas.GetTop(rectangle) - Player.Height);
						onFloor = true;
					}
				}
			}
		}

		private void BackgroundParallax()
		{
			var backgroundSegments = gameCanvas.Children.OfType<Image>().Where(i => (string)i.Tag == "Background").ToArray();

			// Move each segment a little left to simulate depth
			for(int i = 0; i < backgroundSegments.Length; i++)
			{
				var backgroundSegment = backgroundSegments[i];
				Canvas.SetLeft(backgroundSegment, (Canvas.GetLeft(backgroundSegment) - 0.5 * (i + 1)) % 4400);
			}
		}

		private void PauseChecking()
		{
			// If P was pressed...
			if(!Keyboard.IsKeyDown(Key.P)) return;
			const string caption = "PAUZE";
			const string message = "Wil je stoppen met spelen?";
			const MessageBoxButton buttons = MessageBoxButton.YesNo;

			// ...show a window and return to the main menu if Yes was clicked
			if(MessageBox.Show(message, caption, buttons) != MessageBoxResult.Yes) return;
			MainWindow mainWindow = new();
			mainWindow.Show();
			Close();
		}
	}
}
