using Windows.ApplicationModel.Resources;
using static RLD.UWPCore.KeyDictionary;

namespace RLD.UWPCore
{
	public static class LocalizeService
	{
		public static string Localize(StringKey key) => ResourceLoader.GetForCurrentView().GetString(key.ToString());

	}
}
