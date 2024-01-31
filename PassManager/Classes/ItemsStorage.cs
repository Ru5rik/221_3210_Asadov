using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Windows.Input;

namespace PassManager.Classes
{
	public class ItemsStorage
	{
		private const string _path = "managerData.json";
		private const string _path_s = "managerData.dat";
		public List<ItemModel> Items { get; set; }
		public ItemsStorage()
		{
			Items = new List<ItemModel>();

			Load();
		}
		public void Load()
		{
			//if (File.Exists(_path))
			//{
			//	try
			//	{
			//		Items.AddRange(JsonSerializer.Deserialize<List<ItemModel>>(File.ReadAllText(_path)));
			//	}
			//	catch (Exception)
			//	{

			//	}
			//}

			//Items.AddRange(JsonSerializer.Deserialize<List<ItemModel>>(File.ReadAllText(_path)));


			byte[] bytes = Encoding.UTF8.GetBytes(Properties.Settings.Default.Master);
			byte[] hash = SHA256.HashData(bytes);
			byte[] vector = hash.Take(16).ToArray();

			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = hash;
				aesAlg.IV = vector;

				// Create a decryptor to perform the stream transform.
				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				// Create the streams used for decryption.
				using (MemoryStream msDecrypt = new MemoryStream(File.ReadAllBytes(_path_s)))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{

							// Read the decrypted bytes from the decrypting stream
							// and place them in a string.
							string json = srDecrypt.ReadToEnd();

							Items.AddRange(JsonSerializer.Deserialize<List<ItemModel>>(json));
						}
					}
				}
			}


		}
		public void Save()
		{
			// not safety
			string json = JsonSerializer.Serialize(Items);

			byte[] bytes = Encoding.UTF8.GetBytes(Properties.Settings.Default.Master);
			byte[] hash = SHA256.HashData(bytes);
			byte[] vector = hash.Take(16).ToArray();
			byte[] encrypted;

			using (Aes aes = Aes.Create())
			{
				byte[] key = aes.Key;
				byte[] iv = aes.IV;
				aes.Key = hash;
				aes.IV = vector;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							//Write all data to the stream.
							swEncrypt.Write(json);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}

			File.WriteAllBytes(_path_s, encrypted);

			//File.WriteAllText(_path, json);
		}
	}
}
