using PassManager.Classes;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace PassManager.Pages
{
	/// <summary>
	/// Interaction logic for StoragePage.xaml
	/// </summary>
	public partial class StoragePage : Page
	{
		private readonly ItemsStorage _storage;
		public ObservableCollection<ItemModel> Items { get; set; }
		public ObservableObject<string> Link { get; set; }
		public ObservableObject<string> Login { get; set; }

		private ItemModel _item;
		/// <summary>
		/// Конструктор страницы с паролями. Принимает объект хранилища.
		/// </summary>
		/// <param name="storage"></param>
		public StoragePage(ItemsStorage storage)
		{
			InitializeComponent();
			_storage = storage;
			_storage.Load();

			Link = new(string.Empty);
			Login = new(string.Empty);

			Items = new ObservableCollection<ItemModel>();
			_storage.Items.ForEach(Items.Add);

			DataContext = this;
		}
		/// <summary>
		/// Поиск элементов по ссылке на сайт.
		/// </summary>
		private void SearchClick(object sender, RoutedEventArgs e)
		{
			Items.Clear();

			_storage.Items.Where(x => x.Link.ToLower().Contains(SearchTextBox.Text)).ToList().ForEach(Items.Add);

			if (string.IsNullOrWhiteSpace(SearchTextBox.Text) && !Items.Any())
			{
				_storage.Items.ForEach(Items.Add);
			}
		}
		private void ClearForm()
		{
			Link.Value = string.Empty;
			Login.Value = string.Empty;
			ItemPass.Password = string.Empty;
		}
		/// <summary>
		/// Отображение панели для добавления нового пароля.
		/// </summary>
		private void AddClick(object sender, RoutedEventArgs e)
		{
			MasterEditor.Visibility = Visibility.Collapsed;
			EditorGrid.Visibility = Visibility.Visible;

			ClearForm();
		}
		/// <summary>
		/// Добавление нового пароля в хранилище.
		/// </summary>
		private void SaveClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;

			if (_item == null)
			{
				MessageBox.Show("Новый пароль добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

				Items.Add(_storage.SetItem(null, Link.Value, Login.Value, ItemPass.Password));
			}
			else
			{
				MessageBox.Show("Данные обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

				var item = _storage.SetItem(_item, Link.Value, Login.Value, ItemPass.Password);
				int index = Items.IndexOf(item);
				Items.RemoveAt(index);
				Items.Insert(index, item);
				_item = null;
			}
			ClearForm();
		}
		/// <summary>
		/// Закрывает панели редактирования. 
		/// </summary>
		private void CancelClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;

			MasterEditor.Visibility = Visibility.Collapsed;
			OldPass.Password = "";
			NewPass.Password = "";
			NewPassSecond.Password = "";
		}
		/// <summary>
		/// Проверка данных нового элемента.
		/// </summary>
		private void ValidateInput(object sender, TextChangedEventArgs e)
		{
			if (string.IsNullOrEmpty(Link.Value) || string.IsNullOrEmpty(Login.Value) || string.IsNullOrEmpty(ItemPass.Password))
			{
				SaveBtn.IsEnabled = false;
			}
			else
			{
				SaveBtn.IsEnabled = true;
			}
		}
		/// <summary>
		/// Обработчик контесткного меню. Также удаляет, копирует и производит настройку поля редактирования.
		/// </summary>
		private void MenuItemClick(object sender, RoutedEventArgs e)
		{
			MenuItem menu = sender as MenuItem;
			string tag = menu.Tag.ToString();
			ItemModel item = menu.DataContext as ItemModel;
			switch (tag)
			{
				case "Edit":
					EditorGrid.Visibility = Visibility.Visible;
					_item = item;
					Link.Value = _item.Link;
					break;
				case "Remove":

					MessageBoxResult answer = MessageBox.Show("Вы действительно хотите удалить эти данные?", "Удаление", MessageBoxButton.YesNo, MessageBoxImage.Question);

					if (answer == MessageBoxResult.Yes)
					{
						_storage.Items.Remove(item);
						Items.Remove(item);
						MessageBox.Show("Пароль удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
					}
					break;
				case "CopyL":
					_storage.CopyLogin(item);
					break;
				case "CopyP":
					_storage.CopyPassword(item);
					break;
				default:
					break;
			}
		}
		/// <summary>
		/// Отображение панели изменения пин кода.
		/// </summary>
		private void EditMasterClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;
			MasterEditor.Visibility = Visibility.Visible;
		}
		/// <summary>
		/// Сохранение пин кода.
		/// </summary>
		private void SaveMasterClick(object sender, RoutedEventArgs e)
		{
			MasterEditor.Visibility = Visibility.Collapsed;
			MessageBox.Show("Пин-код обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			_storage.ChangeMasterKey(NewPass.Password);
		}
		private void LogoutClick(object sender, RoutedEventArgs e)
		{
			_storage.Unload();
			ViewUtils.MainFrame.GoBack();
		}
		/// <summary>
		/// Проверка пин кода.
		/// </summary>
		private void InputPasswordChanged(object sender, RoutedEventArgs e)
		{
			PasswordBox pb = sender as PasswordBox;
			string tag = pb.Tag == null ? string.Empty : pb.Tag.ToString();
			if (tag == "item" && (string.IsNullOrEmpty(Link.Value) || string.IsNullOrEmpty(Login.Value) || string.IsNullOrEmpty(ItemPass.Password)))
			{
				SaveBtn.IsEnabled = false;
			}
			else
			{
				SaveBtn.IsEnabled = true;
			}
			if (OldPass.Password == NewPass.Password || OldPass.Password != Properties.Settings.Default.Master ||
				string.IsNullOrWhiteSpace(OldPass.Password) || string.IsNullOrWhiteSpace(NewPass.Password) || NewPass.Password != NewPassSecond.Password)
			{
				SaveMasterBtn.IsEnabled = false;
			}
			else
			{
				SaveMasterBtn.IsEnabled = true;
			}
		}

		private void ItemMenuClick(object sender, RoutedEventArgs e)
		{
			//...
		}
	}
}
