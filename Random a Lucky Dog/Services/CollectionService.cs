using System.Collections.ObjectModel;

namespace RLD.Services
{
	internal static class CollectionService
	{
		internal static Collection<T> MergeCollections<T>(params Collection<T>[] collections)
		{
			Collection<T> mergedCollection = new Collection<T>();
			foreach (var collection in collections)
			{
				foreach (var item in collection) mergedCollection.Add(item);
			}
			return mergedCollection;
		}
	}
}
