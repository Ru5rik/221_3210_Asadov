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
		public ObservableObject<ItemModel> NewItem { get; set; }
		private ItemModel editable;
		/// <summary>
		/// Конструктор страницы с паролями. Принимает объект хранилища.
		/// </summary>
		/// <param name="storage"></param>
		public StoragePage(ItemsStorage storage)
		{
			InitializeComponent();
			_storage = storage;
			_storage.Load();
			Items = new ObservableCollection<ItemModel>();
			NewItem = new ObservableObject<ItemModel>(new ItemModel());

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
		/// <summary>
		/// Отображение панели для добавления нового пароля.
		/// </summary>
		private void AddClick(object sender, RoutedEventArgs e)
		{
			MasterEditor.Visibility = Visibility.Collapsed;
			EditorGrid.Visibility = Visibility.Visible;

			NewItem.Value = new ItemModel();
		}
		/// <summary>
		/// Добавление нового пароля в хранилище.
		/// </summary>
		private void SaveClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;

			if (editable != null)
			{
				MessageBox.Show("Данные обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

				editable.Copy(NewItem.Value); // fix
				int index = Items.IndexOf(editable);
				Items.RemoveAt(index);
				Items.Insert(index, editable);

				editable = null;
			}
			else
			{
				MessageBox.Show("Новый пароль добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

				ItemModel item = new ItemModel();
				item.Copy(NewItem.Value);

				_storage.Items.Add(item);
				Items.Add(item);
			}
			NewItem.Value.Clear();
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
			if (NewItem.Value.IsValidate)
			{
				SaveBtn.IsEnabled = true;
			}
			else
			{
				SaveBtn.IsEnabled = false;
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
					editable = item;
					NewItem.Value.Copy(editable);
					NewItem.OnPropChanged("Value");
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
					Clipboard.SetText(item.GetLogin());
					break;
				case "CopyP":
					Clipboard.SetText(item.GetPassword());
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
			Properties.Settings.Default.Master = NewPass.Password;
			Properties.Settings.Default.Save();
		}
		/// <summary>
		/// Проверка пин кода.
		/// </summary>
		private void InputPasswordChanged(object sender, RoutedEventArgs e)
		{
			if (OldPass.Password != NewPass.Password && !string.IsNullOrWhiteSpace(OldPass.Password) && !string.IsNullOrWhiteSpace(NewPass.Password) && NewPass.Password == NewPassSecond.Password)
			{
				SaveMasterBtn.IsEnabled = true;
			}
			else
			{
				SaveMasterBtn.IsEnabled = false;
			}
		}

		private void ItemMenuClick(object sender, RoutedEventArgs e)
		{
			//...
		}
	}
}
