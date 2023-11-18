using static RLD.CPCore.Helpers.Security;

namespace RLD.CPCore.Models
{
	public readonly struct Device
	{
		readonly string id;
		readonly string password;
		public Device(string id, string password)
		{
			this.id = id;
			string hash1 = GetSHA256("ID:" + id);
			string hash2 = GetSHA256(hash1 + ".Password:" + password);
			this.password = hash2;
		}
	}
}
