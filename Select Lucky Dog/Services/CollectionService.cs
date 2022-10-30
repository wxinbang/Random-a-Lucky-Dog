using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Select_Lucky_Dog.Services
{
    internal static class CollectionService
    {
        internal static Collection<T> MergeCollections<T>(params Collection<T>[] collections)
        {
            Collection<T> mergedCollection = new Collection<T>();
            foreach (var collection in collections)
            {
                foreach(var item in collection)mergedCollection.Add(item);
            }
            return mergedCollection;
        }
    }
}
