using System.Windows;

namespace project_arcade
{
	/// <summary>
	/// Interaction logic for _2spelerkeuze.xaml
	/// </summary>
	public partial class _2spelerkeuze : Window
	{
		public _2spelerkeuze()
		{
			InitializeComponent();
		}

		private void TwoPlayerToMain(object sender, RoutedEventArgs e)
		{
			MainWindow gw = new MainWindow();
			gw.Visibility = Visibility.Visible;
			this.Close();
		}

		private void TwoPlayerToGame(object sender, RoutedEventArgs e)
		{
			// this variable will be used to make the second player apear on screen with the rest of the multiplayer function
			GameWindow gw = new GameWindow(true);
			gw.Visibility = Visibility.Visible;
			this.Close();
		}
	}
}
