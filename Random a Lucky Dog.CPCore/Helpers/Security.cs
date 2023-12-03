using System.Security.Cryptography;
using System.Text;

namespace RLD.CPCore.Helpers
{
	public static class Security
	{
		public static string GetSHA256(string input)
		{
			var hashAlgorithm = SHA256.Create();

			byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

			var sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}
	}
}
