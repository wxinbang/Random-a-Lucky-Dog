using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace RLD.UWPCore.Services
{
	public class DeviceService
	{
		public static async Task<DeviceInformationCollection> GetAllPortableDeviceAsync() => await DeviceInformation.FindAllAsync(DeviceClass.PortableStorageDevice);
	}
}
