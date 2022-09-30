using System.Collections.Generic;

namespace Rezaee.Data.Iranseda.Extensions
{
    /// <summary>
    /// Extension methods for <see cref="List{T}"/>
    /// </summary>
    public static class ListExtensions
    {
        /// <summary>
        /// Based on the internal elements in the list and without considering the order,
        /// it calculates the hash of this <see cref="List{T}"/>.
        /// <!-- Code reference: https://stackoverflow.com/a/670068/11455069 -->
        /// </summary>
        /// <typeparam name="T">The type of elements in the <see cref="List{T}"/>.</typeparam>
        /// <param name="items">Elements that are in the <see cref="List{T}"/></param>
        /// <returns>A hash code for the specified object</returns>
        public static int GetOrderIndependentHashCode<T>(this List<T> items)
        {
            int hash = 0;
            int curHash;
            var valueCounts = new Dictionary<T, int>();

            foreach (T item in items)
            {
                if (item is null)
                {
                    hash = unchecked(hash + 1);
                    continue;
                }

                curHash = EqualityComparer<T>.Default.GetHashCode(item);
                if (valueCounts.TryGetValue(item, out int bitOffset))
                    valueCounts[item] = bitOffset + 1;
                else
                    valueCounts.Add(item, bitOffset);

                hash = unchecked(hash + ((curHash << bitOffset) |
                    (curHash >> (32 - bitOffset))) * 37);
            }

            return hash;
        }
    }
}
