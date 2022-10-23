using System;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace project_arcade
{
	public partial class GameWindow : Window
	{
		#region Variables
		private readonly DispatcherTimer timer = new();

		private const int speed = 10;
		private int gravity;
		private int animationFrame;
		private double lastPlayerTop;
		private bool onFloor;
		private bool paused;
		private bool playing;
		private bool escapeToggle;
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
			PauseChecking();

			if(!paused)
			{
				if(playing)
				{
					AnimatePlayer();

					PlayerMovement();

					PlayerGravity();

					PlayerCollisionDetection();

					PlayerScreenBoundsDetection();
				}

				BackgroundParallax();
			}
		}

		private void PlayerScreenBoundsDetection()
		{
			// Keep the player within view
			if(Canvas.GetLeft(Player) < 0)
			{
				Canvas.SetLeft(Player, 0);
			}
			else if(Canvas.GetLeft(Player) > 1480)
			{
				Canvas.SetLeft(Player, 1480);
			}
		}

		private void AnimatePlayer()
		{
			// Increment the animation frame every tick and roll over every 40 ticks but change the player sprite every 4 ticks
			Player.Source = new BitmapImage(new("/Images/Ninja/Idle/" + ((animationFrame / 4) + 1) + ".png", UriKind.Relative));

			animationFrame = (animationFrame += 1) % 40;
		}

		private void PlayerMovement()
		{
			// Move left if the left arrow key is held and/or right if the right arrow key is held
			if(Keyboard.IsKeyDown(Key.Left))
			{
				Player.RenderTransform = new ScaleTransform(-1, 1);
				Canvas.SetLeft(Player, Canvas.GetLeft(Player) - speed);
			}

			if(Keyboard.IsKeyDown(Key.Right))
			{
				Player.RenderTransform = new ScaleTransform(1, 1);
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

			foreach(var rectangle in ObjectCanvas.Children.OfType<Rectangle>())
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
			var backgroundSegments = BackgroundCanvas.Children.OfType<Image>().Where(i => (string)i.Tag == "Background").ToArray();

			// Move each segment a little left to simulate depth
			for(int i = 0; i < backgroundSegments.Length; i++)
			{
				var backgroundSegment = backgroundSegments[i];
				Canvas.SetLeft(backgroundSegment, (Canvas.GetLeft(backgroundSegment) - 0.5 * (i + 1)) % 4400);
			}
		}

		private void PauseChecking()
		{
			// Toggle the pause menu every time the player presses the desired key
			if(Keyboard.IsKeyDown(Key.Escape))
			{
				if(!escapeToggle)
				{
					escapeToggle = true;
					if(playing)
					{
						paused = !paused;
						PauseCanvas.Visibility = paused ? Visibility.Visible : Visibility.Hidden;
					}
				}
			}
			else
			{
				escapeToggle = false;
			}
		}

		private void ResumeGame()
		{
			paused = false;
			PauseCanvas.Visibility = Visibility.Hidden;
		}

		private void InitializeGameVariables()
		{
			Canvas.SetTop(Player, 675);
			Canvas.SetLeft(Player, 75);
			gravity = 0;
			playing = true;
			StartCanvas.Visibility = Visibility.Hidden;
			ObjectCanvas.Visibility = Visibility.Visible;
		}

		private void StartButton_Click(object sender, RoutedEventArgs e)
		{
			InitializeGameVariables();
		}

		private void ScoresButton_Click(object sender, RoutedEventArgs e)
		{

		}

		private void QuitButton_Click(object sender, RoutedEventArgs e)
		{
			Close();
		}

		private void ResumeButton_Click(object sender, RoutedEventArgs e)
		{
			ResumeGame();
		}

		private void RestartButton_Click(object sender, RoutedEventArgs e)
		{
			InitializeGameVariables();
			ResumeGame();
		}

		private void MenuButton_Click(object sender, RoutedEventArgs e)
		{
			playing = false;
			paused = false;
			StartCanvas.Visibility = Visibility.Visible;
			PauseCanvas.Visibility = Visibility.Hidden;
			ObjectCanvas.Visibility = Visibility.Hidden;
		}
	}
}
