using Windows.ApplicationModel.Resources;
using static Select_Lucky_Dog.Helpers.KeyDictionary;

namespace Select_Lucky_Dog.Services
{
	internal static class LocalizeService
	{
		public static string Localize(StringKey key) => ResourceLoader.GetForCurrentView().GetString(key.ToString());

	}
}
