using System.Text.Json.Serialization;

namespace PassManager.Classes
{
	public class ItemModel
	{
		public string Link { get; set; }
		public string Login { get; set; }
		public string Password { get; set; }
		[JsonIgnore]
		public bool IsValidate => !string.IsNullOrWhiteSpace(Link) && !string.IsNullOrWhiteSpace(Login) && !string.IsNullOrWhiteSpace(Password);
		public void Copy(ItemModel item)
		{
			Link = item.Link;
			Login = item.Login;
			Password = item.Password;
		}
	}
}
