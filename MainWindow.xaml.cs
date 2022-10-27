using System.Windows;

namespace project_arcade
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		public MainWindow()
		{
			InitializeComponent();
		}

		private void One_player_button(object sender, RoutedEventArgs e)
		{
			_1spelerkeuze GW = new _1spelerkeuze();
			GW.Visibility = Visibility.Visible;
			this.Close();
		}

		private void Two_player_button(object sender, RoutedEventArgs e)
		{
			_2spelerkeuze gw = new _2spelerkeuze();
			gw.Visibility = Visibility.Visible;
			this.Close();
		}

		private void Highscores_button(object sender, RoutedEventArgs e)
		{
			HighscoresWindow GW = new HighscoresWindow();
			GW.Visibility = Visibility.Visible;
			this.Close();
		}

		private void Quit_button(object sender, RoutedEventArgs e)
		{
			Application.Current.Shutdown();
		}

	}
}
