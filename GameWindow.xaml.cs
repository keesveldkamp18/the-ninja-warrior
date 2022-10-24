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
		public class Controls
		{
			public Key jump;
			public Key left;
			public Key right;

			public Controls(Key left, Key right, Key jump)
			{
				this.jump = jump;
				this.left = left;
				this.right = right;
			}
		}

		public class Player
		{
			public Image image;
			public Controls controls;

			public int gravity;
			public int animationFrame;
			public double lastTop;
			public bool onFloor;

			public Player(Image image, Controls controls)
			{
				this.image = image;
				this.controls = controls;
			}
		}

		#region Variables
		private readonly DispatcherTimer timer = new();
		private Player[] players = new Player[2];
		
		private const int speed = 10;
		/// <summary>
		/// Controls whether parallax scrolling, player animation, movement, gravity, collision detection and screen boundary detection are enabled.
		/// </summary>
		private bool paused;
		/// <summary>
		/// Controls whether player animation, movement, gravity, collision detection and screen boundary detection are enabled.
		/// </summary>
		private bool playing;
		private bool escapeToggle;
		private bool multiplayer = true;
		#endregion

		public GameWindow()
		{
			InitializeComponent();

			InitializeGameWindow();
		}

		private void InitializeGameWindow()
		{
			players[0] = new(PlayerOne, new(Key.A, Key.D, Key.W));
			players[1] = new(PlayerTwo, new(Key.Left, Key.Right, Key.Up)) { animationFrame = 20 };

			if(!multiplayer)
			{
				PlayerTwo.Visibility = Visibility.Hidden;
			}

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
					for(int i = 0; i < players.Length; i++)
					{
						AnimatePlayer(players[i]);

						if(!multiplayer && i > 0) break;

						MovePlayer(players[i]);

						PlayerGravity(players[i]);

						PlayerCollisionDetection(players[i]);

						PlayerScreenBoundsDetection(players[i]);
					}
				}

				BackgroundParallax();
			}
		}

		private void PlayerScreenBoundsDetection(Player player)
		{
			// Keep the player within view
			if(Canvas.GetLeft(player.image) < 0)
			{
				Canvas.SetLeft(player.image, 0);
			}
			else if(Canvas.GetLeft(player.image) > 1480)
			{
				Canvas.SetLeft(player.image, 1480);
			}
		}

		private void AnimatePlayer(Player player)
		{
			// Increment the animation frame every tick and roll over every 40 ticks but change the player sprite every 4 ticks
			player.image.Source = new BitmapImage(new("/Images/Ninja/Idle/" + ((player.animationFrame / 4) + 1) + ".png", UriKind.Relative));

			player.animationFrame = (player.animationFrame += 1) % 40;
		}

		private void MovePlayer(Player player)
		{
			// Move left if the left arrow key is held and/or right if the right arrow key is held
			if(Keyboard.IsKeyDown(player.controls.left))
			{
				player.image.RenderTransform = new ScaleTransform(-1, 1);
				Canvas.SetLeft(player.image, Canvas.GetLeft(player.image) - speed);
			}

			if(Keyboard.IsKeyDown(player.controls.right))
			{
				player.image.RenderTransform = new ScaleTransform(1, 1);
				Canvas.SetLeft(player.image, Canvas.GetLeft(player.image) + speed);
			}

			// Adds negative force if the spacebar is pressed and if the player is on a floor
			if(Keyboard.IsKeyDown(player.controls.jump) && player.onFloor)
			{
				player.gravity = -20;
				player.onFloor = false;
			}
		}

		private void PlayerGravity(Player player)
		{
			// Makes the player fall down
			player.lastTop = Canvas.GetTop(player.image);
			player.gravity++;
			Canvas.SetTop(player.image, Canvas.GetTop(player.image) + player.gravity);
		}

		private void PlayerCollisionDetection(Player player)
		{
			Rect playerRect = new(Canvas.GetLeft(player.image), Canvas.GetTop(player.image), player.image.Width, player.image.Height);

			foreach(var rectangle in ObjectCanvas.Children.OfType<Rectangle>())
			{
				if((string)rectangle.Tag == "Platform")
				{
					Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

					// Don't let the player fall through a platform if the player was above or on it the previous tick
					if(playerRect.IntersectsWith(platformRect) && player.lastTop + player.image.Height <= Canvas.GetTop(rectangle))
					{
						player.gravity = 0;
						Canvas.SetTop(player.image, Canvas.GetTop(rectangle) - player.image.Height);
						player.onFloor = true;
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

		private void InitializeGameVariables(bool multiplayer)
		{
			for(int i = 0; i < players.Length; i++)
			{
				Canvas.SetTop(players[i].image, 675);
				Canvas.SetLeft(players[i].image, 75 + 200 * i);
				players[i].gravity = 0;
				players[i].image.RenderTransform = new ScaleTransform(1, 1);
			}

			this.multiplayer = multiplayer;
			PlayerTwo.Visibility = multiplayer ? Visibility.Visible : Visibility.Hidden;

			playing = true;
			StartCanvas.Visibility = Visibility.Hidden;
			ObjectCanvas.Visibility = Visibility.Visible;
		}

		private void OnePlayerButton_Click(object sender, RoutedEventArgs e)
		{
			InitializeGameVariables(false);
		}

		private void TwoPlayerButton_Click(object sender, RoutedEventArgs e)
		{
			InitializeGameVariables(true);
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
			InitializeGameVariables(multiplayer);
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
