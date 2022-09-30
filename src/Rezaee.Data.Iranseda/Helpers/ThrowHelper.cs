using System;
using System.Collections.Generic;
using System.Text;

namespace Rezaee.Data.Iranseda.Helpers
{
    /// <summary>
    /// Contains helper methods for throwing exceptions.
    /// </summary>
    public static class ThrowHelper
    {
        /// <summary>
        /// If the value of the argument is null, it will throw an <see cref="ArgumentNullException"/>.
        /// </summary>
        /// <param name="argument">An argument that can be null or object</param>
        /// <param name="paramName">The name of the argument that caused the exception.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public static void ThrowArgumentNullExceptionIfNull(object argument, string? paramName = null)
        {
            if (argument is null)
            {
                if (string.IsNullOrEmpty(paramName))
                    throw new ArgumentNullException();
                else
                    throw new ArgumentNullException(paramName);
            }
        }
    }
}
