using PassManager.Classes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace PassManager.Pages
{
	/// <summary>
	/// Interaction logic for LoginPage.xaml
	/// </summary>
	public partial class LoginPage : Page
	{
		private readonly ItemsStorage _storage;
		public ObservableObject<Visibility> ErrorBlock { get; set; }

		public LoginPage(ItemsStorage storage)
		{
			InitializeComponent();
			_storage = storage;

			ErrorBlock = new ObservableObject<Visibility>(Visibility.Collapsed);

			DataContext = this;
		}
		private void PassBoxKeyDown(object sender, System.Windows.Input.KeyEventArgs e)
		{
			if (e.Key == Key.Return)
			{
				if (PassBox.Password == Properties.Settings.Default.Master)
				{
					_storage.IsAuth = true;
					ViewUtils.MainFrame.Navigate(new StoragePage(_storage));
				}
				else
				{
					ShowError();
				}
			}
		}
		private void ShowError()
		{
			ErrorBlock.Value = Visibility.Visible;

			Task.Run(async () => {
				await Task.Delay(3000);
				ErrorBlock.Value = Visibility.Collapsed;
			});
		}
    }
}
