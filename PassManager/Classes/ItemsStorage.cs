//#define CRYPT

using System.IO;
using System.Text.Json;

namespace PassManager.Classes
{
	public class ItemsStorage
	{
		private const string _path = "managerData.json";
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
#if CRYPT
					string json = SecurityManager.Decrypt(Properties.Settings.Default.Master, File.ReadAllBytes("e_" + _path + ".dat"));
#else
					string json = File.ReadAllText(_path);
#endif
					Items.AddRange(JsonSerializer.Deserialize<List<ItemModel>>(json));
				}
				catch (Exception)
				{

				}
			}
		}
		public void Save()
		{
			string json = JsonSerializer.Serialize(Items);
			File.WriteAllText(_path, json);
#if CRYPT
			File.WriteAllBytes("e_" + _path + ".dat", SecurityManager.Encrypt(Properties.Settings.Default.Master, json));
#endif
		}
	}
}
