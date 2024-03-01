using System.IO;
using System.Text.Json;

namespace PassManager.Classes
{
	public class ItemsStorage
	{
		public bool IsAuth;
		private const string _path = "data.txt";
		public List<ItemModel> Items { get; set; }
		public ItemsStorage()
		{
			Items = new List<ItemModel>();
		}
		public void Load()
		{
			if (File.Exists(_path))
			{
				try
				{
					string json = SecurityManager.Decrypt(Properties.Settings.Default.Master, File.ReadAllBytes(_path));
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
			File.WriteAllBytes(_path, SecurityManager.Encrypt(Properties.Settings.Default.Master, json));
			}
		}
	}
}
