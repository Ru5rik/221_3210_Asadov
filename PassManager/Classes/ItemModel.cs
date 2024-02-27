using System.Text.Json.Serialization;

namespace PassManager.Classes
{
	public class ItemModel
	{
		public string Link { get; set; }
		public byte[] EncryptedLogin { get; set; }
		public byte[] EncryptedPassword { get; set; }
		[JsonIgnore]
		public string Login { get; set; }
		[JsonIgnore]
		public string Password { get; set; }
		[JsonIgnore]
		public bool IsValidate => !string.IsNullOrWhiteSpace(Link) && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);

		public void Copy(ItemModel item)
		{
			Link = item.Link;
			EncryptedLogin = SecurityManager.Encrypt(Link, item.Login);
			EncryptedPassword = SecurityManager.Encrypt(Link, item.Password);
			Login = "";
			Password = "";
		}
		public void Clear()
		{
			Login = "";
			Password = "";
		}
		public string GetLogin()
		{
			return SecurityManager.Decrypt(Link, EncryptedLogin);
		}
		public string GetPassword()
		{
			return SecurityManager.Decrypt(Link, EncryptedPassword);
		}

	}
}
