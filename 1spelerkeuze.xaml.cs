using System.Windows;

namespace project_arcade
{
	/// <summary>
	/// Interaction logic for _1spelerkeuze.xaml
	/// </summary>
	public partial class _1spelerkeuze : Window
	{
		public _1spelerkeuze()
		{
			InitializeComponent();
		}

		private void OnePlayerToMain(object sender, RoutedEventArgs e)
		{
			MainWindow gw = new MainWindow();
			gw.Visibility = Visibility.Visible;
			this.Close();
		}

		private void OnePlayerToGame(object sender, RoutedEventArgs e)
		{
			GameWindow gw = new GameWindow(false);
			gw.Visibility = Visibility.Visible;
			this.Close();
		}
	}
}
