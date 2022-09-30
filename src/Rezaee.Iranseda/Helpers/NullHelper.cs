using System;

namespace Rezaee.Iranseda.Helpers
{
    /// <summary>
    /// The result of comparing two objects in terms of being null or not.
    /// </summary>
    public enum NullComparisonResult
    {
        BothNull,
        OneSideOnly,
        NoneNull,
    }

    /// <summary>
    /// Contains helper methods to deal with nullity.
    /// </summary>
    public class NullHelper
    {
        /// <summary>
        /// Compares whether two objects are null or not.
        /// </summary>
        /// <typeparam name="T">Type of objects to be compared.</typeparam>
        /// <param name="left">The first object to be compared.</param>
        /// <param name="right">The second object to be compared.</param>
        /// <returns>Returns the result as an <see cref="NullComparisonResult"/> object</returns>
        public static NullComparisonResult NullComparison<T>(T left, T right)
        {
            if (ReferenceEquals(left, right))
            {
                if (left == null)
                    return NullComparisonResult.BothNull;
                else
                    return NullComparisonResult.NoneNull;
            }
            else if (left is null || right is null)
                return NullComparisonResult.OneSideOnly;
            else
                return NullComparisonResult.NoneNull;
        }
    }
}
