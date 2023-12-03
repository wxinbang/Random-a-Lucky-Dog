using Windows.ApplicationModel.Resources;
using static RLD.CPCore.KeyDictionary;

namespace RLD.UWPCore.Services
{
	public static class LocalizeService
	{
		public static string Localize(StringKey key) => ResourceLoader.GetForCurrentView().GetString(key.ToString());

	}
}
