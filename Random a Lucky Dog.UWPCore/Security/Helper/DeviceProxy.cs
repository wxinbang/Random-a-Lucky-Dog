using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Foundation;
using static RLD.CPCore.Helpers.Security;

namespace RLD.UWPCore.Helper
{
	public static class DeviceProxy
	{
		public static async Task<DeviceInformationCollection> GetAllPortableDevice() => await DeviceInformation.FindAllAsync(DeviceClass.PortableStorageDevice);
		//public static async 
	}
	public readonly struct Device
	{
		readonly string id;
		readonly string password;
		public Device(string id, string password)
		{
			this.id = id;
			string hash1 = GetSHA256( "ID:" + id);
			string hash2 = GetSHA256(hash1 + ".Password:" + password);
			this.password = hash2;
		}
	}
}
