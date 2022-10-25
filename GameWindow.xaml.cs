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
        private readonly DispatcherTimer timer = new();

        // mijn code
        private const int speed = 10;
        private int gravity1Player;
        private int gravity2Player;
        private double lastPlayer1Top;
        private double lastPlayer2Top;
        private bool player1OnFloor;
        private bool player2OnFloor;

        public bool secondPlayer = false;
        public UIElement Platform2;

        // ! mijn code

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

            CheckMultiPlayer();

            CheckIfPlayerFals();

            PauseChecking();

            BackgroundParallax();
        }

        // mijn code

        private void CheckMultiPlayer()
        {
            if (!secondPlayer)
            {
                gameCanvas.Children.Remove(Player2);
                gameCanvas.Children.Remove(platform1Player2);
                gameCanvas.Children.Remove(platform2Player2);
            }
        }

        private void CheckIfPlayerFals()
        {
            Rect player1Rect = new(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height);
            Rect player2Rect = new(Canvas.GetLeft(Player2), Canvas.GetTop(Player2), Player2.Width, Player2.Height);

            foreach (var rectangle in gameCanvas.Children.OfType<Rectangle>())
            {
                if ((string)rectangle.Tag == "FloorP1")
                {
                    Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

                    // when player hits the floor the game is over ??
                    if (player1Rect.IntersectsWith(platformRect) && lastPlayer1Top + Player1.Height <= Canvas.GetTop(rectangle))
                    {
                        gravity1Player = 0;
                        Canvas.SetTop(Player1, Canvas.GetTop(rectangle) - Player1.Height);
                        MessageBox.Show("Einde spel player 1");
                    }
                }

                if (secondPlayer)
                {
                    if ((string)rectangle.Tag == "FloorP2")
                    {
                        Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

                        // when player hits the floor the game is over ??
                        if (player2Rect.IntersectsWith(platformRect) && lastPlayer2Top + Player1.Height <= Canvas.GetTop(rectangle))
                        {
                            gravity1Player = 0;
                            Canvas.SetTop(Player1, Canvas.GetTop(rectangle) - Player2.Height);
                            MessageBox.Show("Einde spel player 2");
                        }
                    }
                }
            }
        }

        // ! mijn code

        private void PlayerScreenBoundsDetection()
        {
            // Keep the player within view
            if (Canvas.GetLeft(Player1) < 0)
            {
                Canvas.SetLeft(Player1, 0);
            }
            else if (Canvas.GetLeft(Player1) > 1525)
            {
                Canvas.SetLeft(Player1, 1525);
            }

            if (Canvas.GetLeft(Player2) < 0)
            {
                Canvas.SetLeft(Player2, 0);
            }
            else if (Canvas.GetLeft(Player2) > 1525)
            {
                Canvas.SetLeft(Player2, 1525);
            }
        }

        // mijn code

        private void PlayerMovement()
        {
            // Move left if the left arrow key is held and/or right if the right arrow key is held
            if (Keyboard.IsKeyDown(Key.Left))
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) - speed);
            }

            if (Keyboard.IsKeyDown(Key.Right))
            {
                Canvas.SetLeft(Player1, Canvas.GetLeft(Player1) + speed);
            }

            if (Keyboard.IsKeyDown(Key.A))
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) - speed);
            }

            if (Keyboard.IsKeyDown(Key.D))
            {
                Canvas.SetLeft(Player2, Canvas.GetLeft(Player2) + speed);
            }

            // Adds negative force if the jump key is pressed
            if (Keyboard.IsKeyDown(Key.Up) && player1OnFloor)
            {
                gravity1Player = -20;
                player1OnFloor = false;
            }

            if (Keyboard.IsKeyDown(Key.W) && player2OnFloor)
            {
                gravity2Player = -20;
                player2OnFloor = false;
            }
        }

        private void PlayerGravity()
        {
            // Makes the player fall down
            lastPlayer1Top = Canvas.GetTop(Player1);
            gravity1Player++;
            Canvas.SetTop(Player1, Canvas.GetTop(Player1) + gravity1Player);

            lastPlayer2Top = Canvas.GetTop(Player2);
            gravity2Player++;
            Canvas.SetTop(Player2, Canvas.GetTop(Player2) + gravity2Player);
        }

        // !mijn code

        private void PlayerCollisionDetection()
        {
            Rect player1Rect = new(Canvas.GetLeft(Player1), Canvas.GetTop(Player1), Player1.Width, Player1.Height);
            Rect player2Rect = new(Canvas.GetLeft(Player2), Canvas.GetTop(Player2), Player2.Width, Player2.Height);

            foreach (var rectangle in gameCanvas.Children.OfType<Rectangle>())
            {
                if ((string)rectangle.Tag == "Platform")
                {
                    Rect platformRect = new(Canvas.GetLeft(rectangle), Canvas.GetTop(rectangle), rectangle.Width, rectangle.Height);

                    // Don't let the player fall through a platform if the player was above or on it the previous tick
                    if (player1Rect.IntersectsWith(platformRect) && lastPlayer1Top + Player1.Height <= Canvas.GetTop(rectangle))
                    {
                        gravity1Player = 0;
                        Canvas.SetTop(Player1, Canvas.GetTop(rectangle) - Player1.Height);
                        player1OnFloor = true;
                    }

                    if (player2Rect.IntersectsWith(platformRect) && lastPlayer2Top + Player2.Height <= Canvas.GetTop(rectangle))
                    {
                        gravity2Player = 0;
                        Canvas.SetTop(Player2, Canvas.GetTop(rectangle) - Player2.Height);
                        player2OnFloor = true;
                    }
                }
            }
        }

        private void BackgroundParallax()
        {
            var backgroundSegments = gameCanvas.Children.OfType<Image>().Where(i => (string)i.Tag == "Background").ToArray();

            // Move each segment a little left to simulate depth
            for (int i = 0; i < backgroundSegments.Length; i++)
            {
                var backgroundSegment = backgroundSegments[i];
                Canvas.SetLeft(backgroundSegment, (Canvas.GetLeft(backgroundSegment) - 0.5 * (i + 1)) % 4400);
            }
        }

        // mijn code

        private void PauseChecking()
        {
            // If P was pressed...
            if (!Keyboard.IsKeyDown(Key.P)) return;
            const string caption = "PAUZE";
            const string message = "Wil je stoppen met spelen?";
            const MessageBoxButton buttons = MessageBoxButton.YesNo;

            // ...show a window and return to the main menu if Yes was clicked
            if (MessageBox.Show(message, caption, buttons) != MessageBoxResult.Yes) return;
            MainWindow mainWindow = new();
            mainWindow.Show();
            Close();
        }

        // ! mijn code
    }
}