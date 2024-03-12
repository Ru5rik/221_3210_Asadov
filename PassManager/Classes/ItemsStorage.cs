using System.IO;
using System.Text;
using System.Text.Json;
using System.Windows;

namespace PassManager.Classes
{
	public class ItemsStorage
	{
		public bool IsAuth;
		private const string _path = "data.json";
		public List<ItemModel> Items { get; set; }
		public ItemsStorage()
		{
			Items = new List<ItemModel>();
		}
		public void Unload()
		{
			Save();
			IsAuth = false;
			Items.Clear();
		}
		public void Load()
		{
			if (File.Exists(_path))
			{
				try
				{
					string json = SecurityManager.Decrypt(Properties.Settings.Default.Master, Convert.FromHexString(File.ReadAllText(_path)));
					Items.AddRange(JsonSerializer.Deserialize<List<ItemModel>>(json));
				}
				catch (Exception)
				{

				}
			}
		}
		public void Save()
		{
			if (IsAuth)
			{
				string json = JsonSerializer.Serialize(Items);
				File.WriteAllText(_path, Convert.ToHexString(SecurityManager.Encrypt(Properties.Settings.Default.Master, json)));
			}
		}
		public void ChangeMasterKey(string master)
		{
            foreach (var item in Items)
            {
				item.Login = SecurityManager.Recrypt(Properties.Settings.Default.Master, master, item.Login);
				item.Password = SecurityManager.Recrypt(Properties.Settings.Default.Master, master, item.Password);
			}

            Properties.Settings.Default.Master = master;
			Properties.Settings.Default.Save();

		}
		public ItemModel SetItem(ItemModel item, string site, string login, string password)
		{
			if (item == null)
			{
				ItemModel model = new ItemModel();
				model.Link = site;
				model.Login = SecurityManager.Encrypt(Properties.Settings.Default.Master, login);
				model.Password = SecurityManager.Encrypt(Properties.Settings.Default.Master, password);
				Items.Add(model);
				return model;
			}
			else
			{
				item.Link = site;
				item.Login = SecurityManager.Encrypt(Properties.Settings.Default.Master, login);
				item.Password = SecurityManager.Encrypt(Properties.Settings.Default.Master, password);
				return item;
			}
		}
		public void CopyLogin(ItemModel item)
		{
			Clipboard.SetText(GetValue(item.Login));
		}
		public void CopyPassword(ItemModel item)
		{
			Clipboard.SetText(GetValue(item.Password));
		}
		private string GetValue(byte[] bytes)
		{
			Task.Run(async () =>
			{
				await Task.Delay(15_000);
				Clipboard.Clear();

			});
			return SecurityManager.Decrypt(Properties.Settings.Default.Master, bytes);
		}
	}
}
