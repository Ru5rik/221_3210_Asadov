namespace PassManager.Classes
{
	public class ItemModel
	{
		public string Link { get; set; }
		public byte[] Login { get; set; }
		public byte[] Password { get; set; }
		public void Set(string site, string login, string password)
		{
			Link = site;
			Login = SecurityManager.Encrypt(Link, login);
			Password = SecurityManager.Encrypt(Link, password);
		}
		public string GetLogin()
		{
			return SecurityManager.Decrypt(Link, Login);
		}
		public string GetPassword()
		{
			return SecurityManager.Decrypt(Link, Password);
		}
	}
}
