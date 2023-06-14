using Windows.ApplicationModel;

namespace RLD.Services
{
	internal static class VersionManager
	{
		internal static string GetVersion()
		{
			return string.Format("{0}.{1}.{2}.{3}",
				Package.Current.Id.Version.Major,
				Package.Current.Id.Version.Minor,
				Package.Current.Id.Version.Build,
				Package.Current.Id.Version.Revision);
		}
	}
}
