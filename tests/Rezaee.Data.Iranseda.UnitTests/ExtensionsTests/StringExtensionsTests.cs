using Rezaee.Data.Iranseda.Extensions;
using System.Globalization;

namespace Rezaee.Data.Iranseda.UnitTests.ExtensionsTests
{
    public class StringExtensionsTests
    {
        #region Tests for TranslateDigits()
        [Fact]
        public static void TranslateDigits_FromEnglishToPersianDigits_ReturnsPersianizedString()
        {
            // Arrange
            string content = "1234567890";
            CultureInfo english = new("en-GB");
            CultureInfo persian = new("fa-IR");

            // Act
            string translated = content.TranslateDigits(from: english, to: persian);

            // Assert
            Assert.Equal(expected: "۱۲۳۴۵۶۷۸۹۰", actual: translated);
        }

        [Fact]
        public static void TranslateDigits_AllDigitsFromOneCultureToOthers_ReturnsNonDigitsFromDestinationCulture()
        {
            // Arrange
            var allCultures = CultureInfo.GetCultures(CultureTypes.SpecificCultures);
            foreach (CultureInfo from in allCultures)
            {
                // Skip self thrown ArgumentException
                try
                {
                    var fromDigits = from.NumberFormat.NativeDigits;
                    var targets = allCultures.Where(culture => culture != from);

                    foreach (CultureInfo to in targets)
                    {
                        var toDigits = to.NumberFormat.NativeDigits;
                        string content = string.Join(string.Empty, fromDigits);

                        // Act
                        string translated = content.TranslateDigits(from, to);
                        bool containsDestination = translated.Select(c => c.ToString()).Any(c => !toDigits.Contains(c) && fromDigits.Contains(c));

                        // Assert
                        Assert.False(containsDestination);
                    }
                }
                catch (ArgumentException ex)
                {
                    if (!ex.Message.Contains("Native numbers within the"))
                        // Native numbers within the xxx culture are not in the correct format.
                        // It is necessary to follow the format corresponding to 0123456789.
                        throw;
                }
            }
        }

        [Fact]
        public static void TranslateDigits_EmptyStringFromOneCultureToOthers_ReturnsEmptyString()
        {
            // Arrange
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo from in allCultures)
            {
                var targets = allCultures.Where(culture => culture != from);
                foreach (CultureInfo to in targets)
                {
                    string content = string.Empty;

                    // Act
                    string translated = content.TranslateDigits(from, to);

                    // Assert
                    Assert.Empty(translated);
                }
            }
        }

        [Fact]
        public static void TranslateDigits_NullStringFromOneCultureToOthers_ThrowsArgumentNullException()
        {
            // Arrange
            var allCultures = CultureInfo.GetCultures(CultureTypes.AllCultures);
            foreach (CultureInfo from in allCultures)
            {
                var targets = allCultures.Where(culture => culture != from);
                foreach (CultureInfo to in targets)
                {
                    string? content = null;

                    // Act
                    Action action = () => content!.TranslateDigits(from, to);

                    // Assert
                    Assert.Throws<ArgumentNullException>(action);
                }
            }
        }
        #endregion
    }
}
