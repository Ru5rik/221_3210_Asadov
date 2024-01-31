using PassManager.Classes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
	/// Interaction logic for StoragePage.xaml
	/// </summary>
	public partial class StoragePage : Page
	{
		private readonly ItemsStorage _storage;
		public ObservableCollection<ItemModel> Items { get; set; }
		public ObservableObject<ItemModel> NewItem { get; set; }
		private ItemModel editable;
		public StoragePage(ItemsStorage storage)
		{
			InitializeComponent();
			_storage = storage;
			Items = new ObservableCollection<ItemModel>();
			NewItem = new ObservableObject<ItemModel>(new ItemModel());

			_storage.Items.ForEach(Items.Add);

			DataContext = this;
		}

		private void SearchClick(object sender, RoutedEventArgs e)
		{
			Items.Clear();

			_storage.Items.Where(x => x.Link.ToLower().Contains(SearchTextBox.Text)).ToList().ForEach(Items.Add);

			if (string.IsNullOrWhiteSpace(SearchTextBox.Text) && !Items.Any())
			{
				_storage.Items.ForEach(Items.Add);
			}
		}

		private void AddClick(object sender, RoutedEventArgs e)
		{
			MasterEditor.Visibility = Visibility.Collapsed;
			EditorGrid.Visibility = Visibility.Visible;

			NewItem.Value = new ItemModel();
		}

		private void SaveClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;

			if (editable != null)
			{
				MessageBox.Show("Даныне обновлены", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

				editable.Copy(NewItem.Value);
				int index = Items.IndexOf(editable);
				Items.RemoveAt(index);
				Items.Insert(index, editable);

				editable = null;
			}
			else
			{
				MessageBox.Show("Новый пароль добавлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

				ItemModel item = NewItem.Value;

				_storage.Items.Add(item);
				Items.Add(item);
			}

		}

		private void CancelClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;

			MasterEditor.Visibility = Visibility.Collapsed;
			OldPass.Password = "";
			NewPass.Password = "";
			NewPassSecond.Password = "";
		}

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

		private void ItemClick(object sender, MouseButtonEventArgs e)
		{
			Clipboard.SetText(((ItemModel)((Border)sender).DataContext).Password);
		}

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
						MessageBox.Show("пароль удален", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);

					}
					break;
				case "Copy":
					Clipboard.SetText(item.Password);
					break;
				default:
					break;
			}
		}
		private void EditMasterClick(object sender, RoutedEventArgs e)
		{
			EditorGrid.Visibility = Visibility.Collapsed;
			MasterEditor.Visibility = Visibility.Visible;
		}
		private void SaveMasterClick(object sender, RoutedEventArgs e)
		{
			MasterEditor.Visibility = Visibility.Collapsed;
			MessageBox.Show("Пин-код обновлен", "Успех", MessageBoxButton.OK, MessageBoxImage.Information);
			Properties.Settings.Default.Master = NewPass.Password;
			Properties.Settings.Default.Save();
		}

		private void InputPasswordChanged(object sender, RoutedEventArgs e)
		{
			if (OldPass.Password != NewPass.Password && !string.IsNullOrWhiteSpace(NewPass.Password) && NewPass.Password == NewPassSecond.Password)
			{
				SaveMasterBtn.IsEnabled = true;
			}
			else
			{
				SaveMasterBtn.IsEnabled = false;
			}
		}
	}
}
