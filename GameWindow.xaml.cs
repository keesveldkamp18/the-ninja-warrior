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
		DispatcherTimer timer = new DispatcherTimer();

        #region global variables
        private const int speed = 10;
		private DateTime gameStart;
		private bool endGame = false;

        #region player variables
        private int gravity1Player;
        private int gravity2Player;
        private double lastPlayer1Top;
        private double lastPlayer2Top;
        private double player1Score;
        private double player2Score;
        private double player1TimerBonus;
        private double player2TimerBonus;
        private bool player1OnFloor;
        private bool player2OnFloor;
        private bool player1IsDead;
        private bool player2IsDead;
        public bool secondPlayer = false;
        #endregion

        #endregion


        public GameWindow()
		{
			InitializeComponent();

			gameCanvas.Focus();
			timer.Tick += GameEngine;
			timer.Interval = TimeSpan.FromMilliseconds(20);
			timer.Start();

			player1Score = 0;
			player2Score = 0;
			player1IsDead = false;
			player2IsDead = false;
			gameStart = DateTime.Now;
		}

		private void GameEngine(object? sender, EventArgs e)
		{
			PlayerMovement();

			PlayerGravity();

			PlayerCollisionDetection();

			PlayerScreenBoundsDetection();

			CheckMultiPlayer();

			CheckIfPlayerFals();

			PauseChecking();

			ScoreCount();

			CheckPlayerDeath();
		}


		/// <summary>
		/// check if the game starts from multiplayer screen by making secondplayer variable true
		/// if not it hides player 2 from the screen and the score
		/// </summary>
		private void CheckMultiPlayer()
		{

			// Removes assets for the second player if the playes chooses to play with only 1 player
			if(!secondPlayer)
			{
				gameCanvas.Children.Remove(Player2);
				gameCanvas.Children.Remove(platform1Player2);
				gameCanvas.Children.Remove(platform2Player2);
				gameCanvas.Children.Remove(scorePlayer2);
			}
		}

		/// <summary>
		/// if the player hits the floor the game will end for that player.
		/// player 1 and player 2 both have a different floor to fall on.
		/// </summary>

		private void CheckIfPlayerFals()
		{
			Rect player1Rect = new(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height);
			Rect player2Rect = new(Canvas.GetLeft(Player2), Canvas.GetTop(Player2), Player2.Width, Player2.Height);

			foreach(var rectangle in gameCanvas.Children.OfType<Rectangle>())
			{
				if((string)rectangle.Tag == "FloorP1")
				{
					Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

					if(player1Rect.IntersectsWith(platformRect) && lastPlayer1Top + Player1.Height <= Canvas.GetTop(rectangle))
					{
						player1IsDead = true;
						gravity1Player = 0;
						Canvas.SetTop(Player1, Canvas.GetTop(rectangle) - Player1.Height);					
					}
				}

				if(secondPlayer)
				{
					if((string)rectangle.Tag == "FloorP2")
					{
						Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

						if(player2Rect.IntersectsWith(platformRect) && lastPlayer2Top + Player1.Height <= Canvas.GetTop(rectangle))
						{
							player2IsDead = true;
							gravity1Player = 0;
							Canvas.SetTop(Player1, Canvas.GetTop(rectangle) - Player2.Height);							
						}
					}
				}
			}
		}

		private void PlayerScreenBoundsDetection()
		{
			// Keep the player within view
			if(Canvas.GetLeft(Player1) < 0)
			{
				Canvas.SetLeft(Player1, 0);
			}
			else if(Canvas.GetLeft(Player1) > 1525)
			{
				Canvas.SetLeft(Player1, 1525);
			}

			if(Canvas.GetLeft(Player2) < 0)
			{
				Canvas.SetLeft(Player2, 0);
			}
			else if(Canvas.GetLeft(Player2) > 1525)
			{
				Canvas.SetLeft(Player2, 1525);
			}
		}

		private void PlayerMovement()
		{
			// Move left if the left arrow key is held and/or right if the right arrow key is held
			if(Keyboard.IsKeyDown(Key.Left))
			{
				Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - speed);
			}
			if(Keyboard.IsKeyDown(Key.Right))
			{
				Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + speed);
			}

			if(Keyboard.IsKeyDown(Key.A))
			{
				Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) - speed);
			}

			if(Keyboard.IsKeyDown(Key.D))
			{
				Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) + speed);
			}

			// Adds negative force if the jump key is pressed
			// -- because the axis is 0 based and the lower you are the higher the number.
			if(Keyboard.IsKeyDown(Key.Up) && player1OnFloor)
			{
				gravity1Player = -20;
				player1OnFloor = false;
			}

			if(Keyboard.IsKeyDown(Key.W) && player2OnFloor)
			{
				gravity2Player = -20;
				player2OnFloor = false;
			}
		}

		/// <summary>
		/// Makes the player fall down when its up in the air.
		/// ++ because of the axis is 0 based from top to bottom
		/// </summary>
		private void PlayerGravity()
		{
			lastPlayer1Top = Canvas.GetTop(Player1);
			gravity1Player++;
			Canvas.SetTop(Player1, Canvas.GetTop(Player1) + gravity1Player);

			lastPlayer2Top = Canvas.GetTop(Player2);
			gravity2Player++;
			Canvas.SetTop(Player2, Canvas.GetTop(Player2) + gravity2Player);
		}

		/// <summary>
		/// When player hits the platforms the gravity will be zero not positive or negative.
		/// This makes the player stays at the current level on the screen.
		/// </summary>
		private void PlayerCollisionDetection()
		{
			// get player rectangels from the gamecanvas
			Rect player1Rect = new(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height);
			Rect player2Rect = new(Canvas.GetLeft(Player2), Canvas.GetTop(Player2), Player2.Width, Player2.Height);

			foreach(var rectangle in gameCanvas.Children.OfType<Rectangle>())
			{
				if((string)rectangle.Tag == "Platform")
				{
					Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

					if(player1Rect.IntersectsWith(platformRect) && lastPlayer1Top + Player1.Height <= Canvas.GetTop(rectangle))
					{
						gravity1Player = 0;
						Canvas.SetTop(Player1, Canvas.GetTop(rectangle) - Player1.Height);
						player1OnFloor = true;
					}

					if(player2Rect.IntersectsWith(platformRect) && lastPlayer2Top + Player2.Height <= Canvas.GetTop(rectangle))
					{
						gravity2Player = 0;
						Canvas.SetTop(Player2, Canvas.GetTop(rectangle) - Player2.Height);
						player2OnFloor = true;
					}
				}
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

		private void ScoreCount()
		{
            TimeSpan duration = DateTime.Now - gameStart;

            //Increasing of score
            if (!player1IsDead)
			{
				player1TimerBonus = duration.TotalSeconds; 

                if (Keyboard.IsKeyDown(Key.K))
				{
					player1Score += 10;
				}

            }

            if (!player2IsDead)
            {               
                player2TimerBonus = duration.TotalSeconds;

                if (Keyboard.IsKeyDown(Key.L))
				{
                    player2Score += 10;
                }				
            }

			scorePlayer1.Content = "Score: "+ Math.Round(player1Score + player1TimerBonus);
			scorePlayer2.Content = "Score: "+ Math.Round(player2Score + player2TimerBonus);
        }

		private void CheckPlayerDeath()
		{
			if (secondPlayer)
			{
                if (player1IsDead)
                {
                    gameCanvas.Children.Remove(Player1);
                    
                }

                if (player2IsDead)
                {
                    gameCanvas.Children.Remove(Player2);                
                }

				if (player1IsDead && player2IsDead)
				{
					if (!endGame)
					{
                        if (MessageBox.Show("Player 1 score: " + Math.Round(player1Score + player1TimerBonus) + "\n" + "Player 2 score: " + Math.Round(player2Score + player2TimerBonus) + "\n \n" + "Would you like to submit your scores to the highscore leaderboard?", "Game Over!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
							SubmitScore();

                            MainWindow mw = new MainWindow();
                            mw.Visibility = Visibility.Visible;
                            this.Close();
                        }
                        else
                        {
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            this.Close();
                        };

                        endGame = true;
                    }
                    
                }
            }
			else
			{
                if (player1IsDead)
                {
                    if (!endGame)
                    {
                        if (MessageBox.Show("Player 1 score: " + Math.Round(player1Score + player1TimerBonus) + "\n \n" + "Would you like to submit your scores to the highscore leaderboard?", "Game Over!", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                        {
							SubmitScore();

                            MainWindow mw = new MainWindow();
                            mw.Visibility = Visibility.Visible;
                            this.Close();
                        }
                        else
                        {
                            MainWindow mw = new MainWindow();
                            mw.Show();
                            this.Close();
                        };

                        endGame = true;
                    }

                }              
            }			
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
            addScore.Parameters.AddWithValue("@score", Math.Round(player1Score + player1TimerBonus));
            addScore.Connection = connection;
            addScore.ExecuteNonQuery();

			//Submit player 2 score
            if (secondPlayer)
			{
                addScore.CommandText = "INSERT INTO scores (playerName, playerScore) VALUES ('Player 2', @score2)";
                addScore.Parameters.AddWithValue("@score2", Math.Round(player2Score + player2TimerBonus));
                addScore.Connection = connection;
                addScore.ExecuteNonQuery();
            }
               
            //close connection
            connection.Close();

        }
	}
}
