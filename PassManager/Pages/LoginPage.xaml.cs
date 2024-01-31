using PassManager.Classes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

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

		private void ButtonClick(object sender, RoutedEventArgs e)
		{
			if (PassBox.Password == Properties.Settings.Default.Master)
			{
				ViewUtils.MainFrame.Navigate(new StoragePage(_storage));
			}
			else
			{
				ErrorBlock.Value = Visibility.Visible;
			}
		}

	}
}
