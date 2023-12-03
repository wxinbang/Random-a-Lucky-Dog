using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Windows.Storage;

namespace RLD.UWPCore.Services
{
	public static class IdentityService
	{
		public static async Task<bool> VerifyIdentityAsync()
		{
#if DEBUG
			return true;
#else
			var Folders = await KnownFolders.RemovableDevices.GetFoldersAsync();
			foreach (var folder in Folders)
			{
				try
				{
					var file = await folder.GetFileAsync("IdentityFile");
					IList<string> contents = await FileIO.ReadLinesAsync(file);
					using (SHA256 sha256Hash = SHA256.Create())
					{
						string hash = GetHash(sha256Hash, "User:" + contents[0]);
						Debug.WriteLine(hash);
						if (hash == contents[1]) return true;
					}
				}
				catch {; }
			}
			return false;
#endif
		}
		public static async Task<bool> VerifyIdentityAsync(string password)
		{
			var Folders = await KnownFolders.RemovableDevices.GetFoldersAsync();
			foreach (var folder in Folders)
			{
				try
				{
					var file = await folder.GetFileAsync("IdentityFile");
					IList<string> contents = await FileIO.ReadLinesAsync(file);
					using (SHA256 sha256Hash = SHA256.Create())
					{
						string hash = GetHash(sha256Hash, contents[1] + ".Password:" + password);
						Debug.WriteLine(hash);
						if (hash == contents[2]) return true;
					}
				}
				catch {; }
			}
			return false;
		}

		private static string GetHash(HashAlgorithm hashAlgorithm, string input)
		{
			byte[] data = hashAlgorithm.ComputeHash(Encoding.UTF8.GetBytes(input));

			var sBuilder = new StringBuilder();

			for (int i = 0; i < data.Length; i++) sBuilder.Append(data[i].ToString("x2"));

			return sBuilder.ToString();
		}
	}
}
