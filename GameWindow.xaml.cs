using MySql.Data.MySqlClient;
using System;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Shapes;
using System.Windows.Threading;
using static System.Formats.Asn1.AsnWriter;

namespace project_arcade
{
	public partial class GameWindow : Window
	{
		private readonly DispatcherTimer timer = new DispatcherTimer();

		private const int speed = 10;

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

		private DateTime gameStartTime;

		private double platformSpeed = 4.0;

		private bool gameOver;
		private bool secondPlayer;

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

			PauseChecking();
		}

		private void PlayerMovement()
		{
			// Move left if the left arrow key is held and/or right if the right arrow key is held
			if(Keyboard.IsKeyDown(Key.Left))
			{
				Canvas.SetLeft(player1, Canvas.GetLeft(player1) - speed);
			}

			if(Keyboard.IsKeyDown(Key.Right))
			{
				Canvas.SetLeft(player1, Canvas.GetLeft(player1) + speed);
			}

			if(Keyboard.IsKeyDown(Key.A))
			{
				Canvas.SetLeft(player2, Canvas.GetLeft(player2) - speed);
			}

			if(Keyboard.IsKeyDown(Key.D))
			{
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

		// Makes the players fall down
		private void PlayerGravity()
		{
			lastTopPlayer1 = Canvas.GetTop(player1);
			gravityPlayer1++;
			Canvas.SetTop(player1, Canvas.GetTop(player1) + gravityPlayer1);

			lastTopPlayer2 = Canvas.GetTop(player2);
			gravityPlayer2++;
			Canvas.SetTop(player2, Canvas.GetTop(player2) + gravityPlayer2);
		}

		private void PlayerCollisionDetection()
		{
			onFloorPlayer1 = false;
			onFloorPlayer2 = false;

			Rect player1Rect = new(Canvas.GetLeft(player1), Canvas.GetTop(player1), player1.Width, player1.Height);
			Rect player2Rect = new(Canvas.GetLeft(player2), Canvas.GetTop(player2), player2.Width, player2.Height);

			foreach(var rectangle in gameCanvas.Children.OfType<Rectangle>().Where(p => (string)p.Tag == "Platform"))
			{
				Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

				// Don't let the player fall through a platform if the player was above or on it the previous tick
				if(player1Rect.IntersectsWith(platformRect) && lastTopPlayer1 + player1.Height <= Canvas.GetTop(rectangle))
				{
					gravityPlayer1 = 0;
					Canvas.SetTop(player1, Canvas.GetTop(rectangle) - player1.Height);
					onFloorPlayer1 = true;
					canJumpPlayer1 = true;
				}

				if(player2Rect.IntersectsWith(platformRect) && lastTopPlayer2 + player2.Height <= Canvas.GetTop(rectangle))
				{
					gravityPlayer2 = 0;
					Canvas.SetTop(player2, Canvas.GetTop(rectangle) - player2.Height);
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
			else if(Canvas.GetLeft(player1) > 1525)
			{
				Canvas.SetLeft(player1, 1525);
			}

			if(Canvas.GetLeft(player2) < 0)
			{
				Canvas.SetLeft(player2, 0);
			}
			else if(Canvas.GetLeft(player2) > 1525)
			{
				Canvas.SetLeft(player2, 1525);
			}
		}

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
						Canvas.SetTop(player1, Canvas.GetTop(rectangle) - player1.Height);
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
							Canvas.SetTop(player1, Canvas.GetTop(rectangle) - player2.Height);
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
			foreach(var platform in gameCanvas.Children.OfType<Rectangle>().Where(p => (string)p.Tag == "Platform"))
			{
				Canvas.SetLeft(platform, Canvas.GetLeft(platform) - platformSpeed);

				if(Canvas.GetLeft(platform) <= -500)
				{
					Canvas.SetLeft(platform, Canvas.GetLeft(platform) + 2400);

					scorePlayer1 += isDeadPlayer1 ? 0 : 1;
					scorePlayer2 += isDeadPlayer2 ? 0 : 1;
				}
			}

			platformSpeed += 0.01;
		}

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
			addScore.CommandText = "INSERT INTO scores (playerName, playerScore) VALUES ('Player 1', @score)";
			addScore.Parameters.AddWithValue("@score", Math.Round(scorePlayer1 + timerBonusPlayer1));
			addScore.Connection = connection;
			addScore.ExecuteNonQuery();

			//Submit player 2 score
			if(secondPlayer)
			{
				addScore.CommandText = "INSERT INTO scores (playerName, playerScore) VALUES ('Player 2', @score2)";
				addScore.Parameters.AddWithValue("@score2", Math.Round(scorePlayer2 + timerBonusPlayer2));
				addScore.Connection = connection;
				addScore.ExecuteNonQuery();
			}

			//close connection
			connection.Close();

		}
	}
}
