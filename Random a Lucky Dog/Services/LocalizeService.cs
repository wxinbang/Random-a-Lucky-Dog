using Windows.ApplicationModel.Resources;
using static RLD.Helpers.KeyDictionary;

namespace RLD.Services
{
	internal static class LocalizeService
	{
		public static string Localize(StringKey key) => ResourceLoader.GetForCurrentView().GetString(key.ToString());

	}
}
