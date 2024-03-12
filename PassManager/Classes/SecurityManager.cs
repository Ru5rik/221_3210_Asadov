using System.IO;
using System.Security.Cryptography;
using System.Text;
using System.Windows.Input;

namespace PassManager.Classes
{
	public static class SecurityManager
	{
		public static byte[] Recrypt(string oldKey, string newKey, byte[] value)
		{
			return Encrypt(newKey, Decrypt(oldKey, value));
		}
		private static byte[] GetStingHash(string key)
		{
			return SHA256.HashData(Encoding.UTF8.GetBytes(key));
		}
		public static byte[] Encrypt(string key, string value)
		{
			byte[] keyBytes = GetStingHash(key);
			byte[] vectorBytes = keyBytes.Take(16).ToArray();
			byte[] encrypted;

			using (Aes aes = Aes.Create())
			{
				aes.Key = keyBytes;
				aes.IV = vectorBytes;

				ICryptoTransform encryptor = aes.CreateEncryptor(aes.Key, aes.IV);

				using (MemoryStream msEncrypt = new MemoryStream())
				{
					using (CryptoStream csEncrypt = new CryptoStream(msEncrypt, encryptor, CryptoStreamMode.Write))
					{
						using (StreamWriter swEncrypt = new StreamWriter(csEncrypt))
						{
							swEncrypt.Write(value);
						}
						encrypted = msEncrypt.ToArray();
					}
				}
			}
			return encrypted;
		}
		public static string Decrypt(string key, byte[] value)
		{
			File.WriteAllText("bytes.txt", Convert.ToHexString(value));

			byte[] keyBytes = GetStingHash(key);
			byte[] vectorBytes = keyBytes.Take(16).ToArray();

			string decrypted = string.Empty;

			using (Aes aesAlg = Aes.Create())
			{
				aesAlg.Key = keyBytes;
				aesAlg.IV = vectorBytes;

				ICryptoTransform decryptor = aesAlg.CreateDecryptor(aesAlg.Key, aesAlg.IV);

				using (MemoryStream msDecrypt = new MemoryStream(value))
				{
					using (CryptoStream csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
					{
						using (StreamReader srDecrypt = new StreamReader(csDecrypt))
						{
							decrypted = srDecrypt.ReadToEnd();
						}
					}
				}
			}
			File.WriteAllText("dec.txt", decrypted);

			return decrypted;
		}
	}
}
