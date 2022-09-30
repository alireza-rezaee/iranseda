using Rezaee.Iranseda.Helpers;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Rezaee.Iranseda.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Changing the digits in a string from a language to another.
        /// Non-numeric characters will be skipped.
        /// <example>
        /// For example:
        /// <code>
        /// CultureInfo english = new("en-GB");
        /// CultureInfo persian = new("fa-IR");
        /// string translated = "نوروز 1401".TranslateDigits(from: english, to: persian)
        /// </code>
        /// Results in <c>translated</c>'s having the value "نوروز ۱۴۰۱".
        /// </example>
        /// </summary>
        /// <param name="content">The text that contains digits</param>
        /// <param name="from">The source language</param>
        /// <param name="to">The target language</param>
        /// <returns>Translated string using target language digits.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        public static string TranslateDigits(this string content, CultureInfo from, CultureInfo to)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(from, nameof(from));
            ThrowHelper.ThrowArgumentNullExceptionIfNull(to, nameof(to));

            if (content == null)
                throw new ArgumentNullException(nameof(content));
            else if (content == string.Empty)
                return content;

            if (from == to)
                return content;

            var fromDigits = from.NumberFormat.NativeDigits;
            var toDigits = to.NumberFormat.NativeDigits;

            if (fromDigits.Distinct().Count() != 10)
                throw new ArgumentException(message: $"Native numbers within the {from.Name} culture are not in the correct format."
                    + " " + "It is necessary to follow the format corresponding to 0123456789.");

            if (fromDigits == toDigits)
                return content;

            var toTranslates = content.Distinct().Select(c => c.ToString()).Where(c => fromDigits.Contains(c));

            var convertion = Enumerable.Range(0, 10).Select(i => new KeyValuePair<string, string>(key: fromDigits[i], toDigits[i]));
            var convertionDictionary = convertion.ToDictionary(x => x.Key, x => x.Value);

            foreach (string toTranslate in toTranslates)
                content = content.Replace(toTranslate, convertionDictionary[toTranslate]);

            return content;
        }
    }
}
