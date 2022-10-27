using Microsoft.VisualBasic;
using MySql.Data.MySqlClient;
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
		#region Global Variables
		#region Player Variables
		private int gravityPlayer1;
		private int gravityPlayer2;
		private double lastTopPlayer1;
		private double lastTopPlayer2;
		private double scorePlayer1;
		private double scorePlayer2;
		private double timerBonusPlayer1;
		private double timerBonusPlayer2;
		private bool onFloorPlayer1;
		private bool onFloorPlayer2;
		private bool isDeadPlayer1;
		private bool isDeadPlayer2;
		private bool canJumpPlayer1;
		private bool canJumpPlayer2;
		private string namePlayer1;
		private string namePlayer2;
		private int animationTickPlayer1;
		private int animationTickPlayer2;
		private bool movingPlayer1;
		private bool movingPlayer2;
		#endregion

		private DispatcherTimer timer = new DispatcherTimer();
		private DateTime gameStartTime;
		private const int speed = 10;
		private double platformSpeed = 4.0;
		private bool secondPlayer;
		private bool gameOver;
		#endregion

		public GameWindow(bool secondPlayer)
		{
			this.secondPlayer = secondPlayer;

			InitializeComponent();

			StartGame();
		}

		private void StartGame()
		{
			CheckMultiplayer();

			timer.Tick += GameEngine;
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Start();

			gameStartTime = DateTime.Now;
		}

		/// <summary>
		/// check if the game starts from multiplayer screen by making secondplayer variable true
		/// if not it hides player 2 from the screen and the score
		/// </summary>
		private void CheckMultiplayer()
		{
			// Removes assets for the second player if the player chooses to play with only 1 player
			if(!secondPlayer)
			{
				gameCanvas.Children.Remove(player2);
				gameCanvas.Children.Remove(platform1Player2);
				gameCanvas.Children.Remove(platform2Player2);
				gameCanvas.Children.Remove(platform3Player2);
				gameCanvas.Children.Remove(scoreLabelPlayer2);
			}
		}

		private void GameEngine(object? sender, EventArgs e)
		{
			PlayerMovement();

			PlayerGravity();

			PlayerCollisionDetection();

			PlayerScreenBoundsDetection();

			CheckIfPlayerHitFloor();

			ScoreCount();

			CheckPlayerDeath();

			MovePlatforms();

			AnimatePlayer();

			BackgroundParallax();

			PauseChecking();
		}

		private void PlayerMovement()
		{
			movingPlayer1 = false;

			// Move left if the left arrow key is held and/or right if the right arrow key is held
			if(Keyboard.IsKeyDown(Key.Left))
			{
				movingPlayer1 = true;
				player1.Children.OfType<Image>().First().RenderTransform = new ScaleTransform(-1, 1);
				Canvas.SetLeft(player1, Canvas.GetLeft(player1) - speed);
			}

			if(Keyboard.IsKeyDown(Key.Right))
			{
				movingPlayer1 = !movingPlayer1;
				player1.Children.OfType<Image>().First().RenderTransform = new ScaleTransform(1, 1);
				Canvas.SetLeft(player1, Canvas.GetLeft(player1) + speed);
			}

			movingPlayer2 = false;

			if(Keyboard.IsKeyDown(Key.A))
			{
				movingPlayer2 = true;
				player2.Children.OfType<Image>().First().RenderTransform = new ScaleTransform(-1, 1);
				Canvas.SetLeft(player2, Canvas.GetLeft(player2) - speed);
			}

			if(Keyboard.IsKeyDown(Key.D))
			{
				movingPlayer2 = !movingPlayer2;
				player2.Children.OfType<Image>().First().RenderTransform = new ScaleTransform(1, 1);
				Canvas.SetLeft(player2, Canvas.GetLeft(player2) + speed);
			}

			// Adds negative force if the jump key is pressed
			if(Keyboard.IsKeyDown(Key.Up) && canJumpPlayer1)
			{
				gravityPlayer1 = -20;
				canJumpPlayer1 = false;
			}

			if(Keyboard.IsKeyDown(Key.W) && canJumpPlayer2)
			{
				gravityPlayer2 = -20;
				canJumpPlayer2 = false;
			}
		}

		/// <summary>
		/// Makes the player fall down when its up in the air.
		/// ++ because of the axis is 0 based from top to bottom
		/// </summary>
		private void PlayerGravity()
		{
			lastTopPlayer1 = Canvas.GetTop(player1);
			gravityPlayer1++;
			Canvas.SetTop(player1, Canvas.GetTop(player1) + gravityPlayer1);

			lastTopPlayer2 = Canvas.GetTop(player2);
			gravityPlayer2++;
			Canvas.SetTop(player2, Canvas.GetTop(player2) + gravityPlayer2);
		}

		/// <summary>
		/// When player hits the platforms the gravity will be zero not positive or negative.
		/// This makes the player stays at the current level on the screen.
		/// </summary>
		private void PlayerCollisionDetection()
		{
			onFloorPlayer1 = false;
			onFloorPlayer2 = false;

			Rect player1Rect = new(Canvas.GetLeft(player1), Canvas.GetTop(player1), player1.Width, player1.Height);
			Rect player2Rect = new(Canvas.GetLeft(player2), Canvas.GetTop(player2), player2.Width, player2.Height);

			foreach(var platform in gameCanvas.Children.OfType<Image>().Where(p => (string)p.Tag == "Platform"))
			{
				Rect platformRect = new(Canvas.GetLeft(platform), Canvas.GetTop(platform), 500, 75);

				// Don't let the player fall through a platform if the player was above or on it the previous tick
				if(player1Rect.IntersectsWith(platformRect) && lastTopPlayer1 + player1.Height <= Canvas.GetTop(platform))
				{
					gravityPlayer1 = 0;
					Canvas.SetTop(player1, Canvas.GetTop(platform) - player1.Height);
					onFloorPlayer1 = true;
					canJumpPlayer1 = true;
				}

				if(player2Rect.IntersectsWith(platformRect) && lastTopPlayer2 + player2.Height <= Canvas.GetTop(platform))
				{
					gravityPlayer2 = 0;
					Canvas.SetTop(player2, Canvas.GetTop(platform) - player2.Height);
					onFloorPlayer2 = true;
					canJumpPlayer2 = true;
				}
			}

			if(onFloorPlayer1)
			{
				Canvas.SetLeft(player1, Canvas.GetLeft(player1) - platformSpeed);
			}

			if(onFloorPlayer2)
			{
				Canvas.SetLeft(player2, Canvas.GetLeft(player2) - platformSpeed);
			}
		}

		private void PlayerScreenBoundsDetection()
		{
			// Keep the player within view
			if(Canvas.GetLeft(player1) < 0)
			{
				Canvas.SetLeft(player1, 0);
			}
			else if(Canvas.GetLeft(player1) > 1600 - player1.Width)
			{
				Canvas.SetLeft(player1, 1600 - player1.Width);
			}

			if(Canvas.GetLeft(player2) < 0)
			{
				Canvas.SetLeft(player2, 0);
			}
			else if(Canvas.GetLeft(player2) > 1600 - player2.Width)
			{
				Canvas.SetLeft(player2, 1600 - player2.Width);
			}
		}

		/// <summary>
		/// if the player hits the floor the game will end for that player.
		/// player 1 and player 2 both have a different floor to fall on.
		/// </summary>
		private void CheckIfPlayerHitFloor()
		{
			Rect player1Rect = new(Canvas.GetLeft(player1), Canvas.GetTop(player1), player1.Width, player1.Height);
			Rect player2Rect = new(Canvas.GetLeft(player2), Canvas.GetTop(player2), player2.Width, player2.Height);

			foreach(var rectangle in gameCanvas.Children.OfType<Rectangle>())
			{
				if((string)rectangle.Tag == "floorPlayer1")
				{
					Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

					// when player hits the floor the game is over ??
					if(player1Rect.IntersectsWith(platformRect) && lastTopPlayer1 + player1.Height <= Canvas.GetTop(rectangle))
					{
						isDeadPlayer1 = true;
					}
				}

				if(secondPlayer)
				{
					if((string)rectangle.Tag == "floorPlayer2")
					{
						Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

						// when player hits the floor the game is over ??
						if(player2Rect.IntersectsWith(platformRect) && lastTopPlayer2 + player1.Height <= Canvas.GetTop(rectangle))
						{
							isDeadPlayer2 = true;
						}
					}
				}
			}
		}

		private void ScoreCount()
		{
			TimeSpan duration = DateTime.Now - gameStartTime;

			//Increasing of score
			if(!isDeadPlayer1)
			{
				timerBonusPlayer1 = duration.TotalSeconds;

				if(Keyboard.IsKeyDown(Key.K))
				{
					scorePlayer1 += 10;
				}

			}

			if(!isDeadPlayer2)
			{
				timerBonusPlayer2 = duration.TotalSeconds;

				if(Keyboard.IsKeyDown(Key.L))
				{
					scorePlayer2 += 10;
				}
			}

			scoreLabelPlayer1.Content = "Score: " + Math.Round(scorePlayer1 + timerBonusPlayer1);
			scoreLabelPlayer2.Content = "Score: " + Math.Round(scorePlayer2 + timerBonusPlayer2);
		}

		private void CheckPlayerDeath()
		{
			if(secondPlayer)
			{
				if(isDeadPlayer1)
				{
					gameCanvas.Children.Remove(player1);
				}

				if(isDeadPlayer2)
				{
					gameCanvas.Children.Remove(player2);
				}

				if(isDeadPlayer1 && isDeadPlayer2)
				{
					if(!gameOver)
					{
						if(MessageBox.Show("Player 1 score: " + Math.Round(scorePlayer1 + timerBonusPlayer1) + "\n" + "Player 2 score: " + Math.Round(scorePlayer2 + timerBonusPlayer2) + "\n \n" + "Would you like to submit your scores to the highscore leaderboard?", "Game Over!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							namePlayer1 = Interaction.InputBox("Please enter the name of Player 1.", "Enter name", "Player 1");
							namePlayer2 = Interaction.InputBox("Please enter the name of Player 2.", "Enter name", "Player 2");
							SubmitScore();

							MainWindow mw = new MainWindow();
							mw.Visibility = Visibility.Visible;
							Close();
						}
						else
						{
							MainWindow mw = new MainWindow();
							mw.Show();
							Close();
						};

						gameOver = true;
					}

				}
			}
			else
			{
				if(isDeadPlayer1)
				{
					if(!gameOver)
					{
						if(MessageBox.Show("Player 1 score: " + Math.Round(scorePlayer1 + timerBonusPlayer1) + "\n \n" + "Would you like to submit your scores to the highscore leaderboard?", "Game Over!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
						{
							namePlayer1 = Interaction.InputBox("Please enter the name of Player 1.", "Enter name", "Player 1");
							SubmitScore();

							MainWindow mw = new MainWindow();
							mw.Visibility = Visibility.Visible;
							Close();
						}
						else
						{
							MainWindow mw = new MainWindow();
							mw.Show();
							Close();
						};

						gameOver = true;
					}
				}
			}
		}

		private void MovePlatforms()
		{
			foreach(var platform in gameCanvas.Children.OfType<Image>().Where(p => (string)p.Tag == "Platform"))
			{
				Canvas.SetLeft(platform, Canvas.GetLeft(platform) - platformSpeed);

				if(Canvas.GetLeft(platform) <= -500)
				{
					Canvas.SetLeft(platform, Canvas.GetLeft(platform) + 2400);

					scorePlayer1 += isDeadPlayer1 ? 0 : 1;
					scorePlayer2 += isDeadPlayer2 ? 0 : 1;
				}
			}

			platformSpeed += 0.0025;
		}

		private void AnimatePlayer()
		{
			// Increment the animation frame every tick and roll over every 40 ticks but change the player sprite every 4 ticks
			if(!onFloorPlayer1)
			{
				if(!player1.Children.OfType<Image>().First().Source.ToString().Contains("Jump"))
				{
					animationTickPlayer1 = 0;
				}

				player1.Children.OfType<Image>().First().Source = new BitmapImage(new("Images/Ninja/Jump/" + animationTickPlayer1 / 4 + ".png", UriKind.Relative));
				Canvas.SetLeft(player1.Children.OfType<Image>().First(), -15);
				if(animationTickPlayer1 < 39)
				{
					animationTickPlayer1++;
				}
			}
			else if(movingPlayer1)
			{
				if(!player1.Children.OfType<Image>().First().Source.ToString().Contains("Run"))
				{
					animationTickPlayer1 = 0;
				}

				player1.Children.OfType<Image>().First().Source = new BitmapImage(new("Images/Ninja/Run/" + animationTickPlayer1 / 4 + ".png", UriKind.Relative));
				Canvas.SetLeft(player1.Children.OfType<Image>().First(), -20);
				animationTickPlayer1 = (animationTickPlayer1 += 1) % 40;
			}
			else
			{
				if(!player1.Children.OfType<Image>().First().Source.ToString().Contains("Idle"))
				{
					animationTickPlayer1 = 0;
				}

				player1.Children.OfType<Image>().First().Source = new BitmapImage(new("Images/Ninja/Idle/" + animationTickPlayer1 / 4 + ".png", UriKind.Relative));
				Canvas.SetLeft(player1.Children.OfType<Image>().First(), -0);
				animationTickPlayer1 = (animationTickPlayer1 += 1) % 40;
			}

			if(!onFloorPlayer2)
			{
				if(!player2.Children.OfType<Image>().First().Source.ToString().Contains("Jump"))
				{
					animationTickPlayer2 = 0;
				}

				player2.Children.OfType<Image>().First().Source = new BitmapImage(new("Images/Ninja/Jump/" + animationTickPlayer2 / 4 + ".png", UriKind.Relative));
				Canvas.SetLeft(player2.Children.OfType<Image>().First(), -15);
				if(animationTickPlayer2 < 39)
				{
					animationTickPlayer2++;
				}
			}
			else if(movingPlayer2)
			{
				if(!player2.Children.OfType<Image>().First().Source.ToString().Contains("Run"))
				{
					animationTickPlayer2 = 0;
				}

				player2.Children.OfType<Image>().First().Source = new BitmapImage(new("Images/Ninja/Run/" + animationTickPlayer2 / 4 + ".png", UriKind.Relative));
				Canvas.SetLeft(player2.Children.OfType<Image>().First(), -20);
				animationTickPlayer2 = (animationTickPlayer2 += 1) % 40;
			}
			else
			{
				if(!player2.Children.OfType<Image>().First().Source.ToString().Contains("Idle"))
				{
					animationTickPlayer2 = 0;
				}

				player2.Children.OfType<Image>().First().Source = new BitmapImage(new("Images/Ninja/Idle/" + animationTickPlayer2 / 4 + ".png", UriKind.Relative));
				Canvas.SetLeft(player2.Children.OfType<Image>().First(), -0);
				animationTickPlayer2 = (animationTickPlayer2 += 1) % 40;
			}
		}

		private void BackgroundParallax()
		{
			var backgroundSegments = gameCanvas.Children.OfType<Image>().Where(b => (string)b.Tag == "Background").ToArray();

			// Move each segment a little left to simulate depth
			for(int i = 0; i < backgroundSegments.Length; i++)
			{
				var backgroundSegment = backgroundSegments[i];
				Canvas.SetLeft(backgroundSegment, (Canvas.GetLeft(backgroundSegment) - ((i + 1) * 0.5) - (platformSpeed - 4)) % 4400);
			}
		}

		/// <summary>
		/// When player hits the p key a message box will pop up and ask the user if it wants to continue or ends the game.
		/// </summary>
		private void PauseChecking()
		{
			// If P was pressed...
			if(!Keyboard.IsKeyDown(Key.P)) return;
			const string caption = "PAUZE";
			const string message = "Wil je ophouden met spelen ??";
			const MessageBoxButton buttons = MessageBoxButton.YesNo;
			if(MessageBox.Show(message, caption, buttons) != MessageBoxResult.Yes) return;
			// OK code
			MainWindow mainWindow = new();
			mainWindow.Show();
			Close();
		}

		private void SubmitScore()
		{
			//might need to download and reference the connector. Download: https://dev.mysql.com/downloads/connector/net/ 
			string connectionString = "SERVER=web0113.zxcs.nl,3306; DATABASE=u45926p46412_highscore; UID=u45926p46412_highscore; PASSWORD=ninjaEndlessRunner;";

			//Connect to DB
			MySqlConnection connection = new MySqlConnection(connectionString);

			//open connection
			connection.Open();

			//Add score to DB
			MySqlCommand addScore = new MySqlCommand();

			//Submit player 1 score
			addScore.CommandText = "INSERT INTO scores (playerName, playerScore) VALUES (@player1name, @score)";
			addScore.Parameters.AddWithValue("@score", Math.Round(scorePlayer1 + timerBonusPlayer1));
			addScore.Parameters.AddWithValue("@player1name", namePlayer1);
			addScore.Connection = connection;
			addScore.ExecuteNonQuery();

			//Submit player 2 score
			if(secondPlayer)
			{
				addScore.CommandText = "INSERT INTO scores (playerName, playerScore) VALUES (@player2name, @score2)";
				addScore.Parameters.AddWithValue("@score2", Math.Round(scorePlayer2 + timerBonusPlayer2));
				addScore.Parameters.AddWithValue("@player2name", namePlayer2);
				addScore.Connection = connection;
				addScore.ExecuteNonQuery();
			}

			//close connection
			connection.Close();
		}
	}
}
