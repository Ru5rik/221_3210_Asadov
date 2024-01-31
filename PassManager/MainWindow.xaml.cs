using PassManager.Classes;
using PassManager.Pages;
using System.ComponentModel;
using System.Windows;

namespace PassManager
{
	/// <summary>
	/// Interaction logic for MainWindow.xaml
	/// </summary>
	public partial class MainWindow : Window
	{
		private readonly ItemsStorage _storage;
		public MainWindow()
		{
			InitializeComponent();

			_storage = new ItemsStorage();

			//if (System.Diagnostics.Debugger.IsAttached)
			//{
			//	MessageBox.Show("Обнаружен отладчик!", "Ошибка", MessageBoxButton.OK, MessageBoxImage.Error);
			//	Close();
			//}
			//else
			//{
			//	ViewUtils.MainFrame = MainFrame;
			//	ViewUtils.MainFrame.Navigate(new LoginPage());
			//}



			ViewUtils.MainFrame = MainFrame;
			ViewUtils.MainFrame.Navigate(new LoginPage(_storage));
		}

		protected override void OnClosing(CancelEventArgs e)
		{
			_storage.Save();

			base.OnClosing(e);
		}
	}
}