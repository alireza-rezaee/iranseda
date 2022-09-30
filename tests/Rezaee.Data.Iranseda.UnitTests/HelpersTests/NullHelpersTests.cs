using Rezaee.Data.Iranseda.Helpers;

namespace Rezaee.Data.Iranseda.UnitTests.HelpersTests
{
    public class NullHelpersTests
    {
        [Fact]
        public static void ThrowIfNull_WhenNullAndHasNotParamName_ThrowsArgumentNullException()
        {
            // Arrange
            object? arg = null;

            // Act
            void action() => ThrowHelper.ThrowArgumentNullExceptionIfNull(arg!);

            // Assert
            Assert.Throws<ArgumentNullException>(action);
        }

        [Fact]
        public static void ThrowIfNull_WhenNullAndHasParamName_ThrowsArgumentNullException()
        {
            // Arrange
            object? arg = null;

            // Act
            string? paramName = nameof(arg);
            void action() => ThrowHelper.ThrowArgumentNullExceptionIfNull(arg!, paramName);

            // Assert
            Assert.Throws<ArgumentNullException>(paramName, action);
        }

        [Fact]
        public static void ThrowIfNull_WhenObject_DoesNotThrowArgumentNullException()
        {
            // Arrange
            object arg = new();

            // Act
            void action() => ThrowHelper.ThrowArgumentNullExceptionIfNull(arg);

            // Assert
            var ex = Record.Exception(action);
            Assert.Null(ex);
        }

        [Fact]
        public static void GetNullStatus_WhenObjectVsObject_ReturnsNoneNullStatus()
        {
            // Arrange
            object obj1 = new();
            object obj2 = new();

            // Act
            var result = NullHelper.NullComparison(obj1, obj2);

            // Assert
            Assert.Equal(NullComparisonResult.NoneNull, result);
        }

        [Fact]
        public static void GetNullStatus_WhenObjectVsNull_ReturnsOneSideOnlyNullStatus()
        {
            // Arrange
            object obj = new();
            object? nonObj = null;

            // Act
            var result1 = NullHelper.NullComparison(obj, nonObj);
            var result2 = NullHelper.NullComparison(nonObj, obj);

            // Assert
            Assert.Equal(NullComparisonResult.OneSideOnly, result1);
            Assert.Equal(NullComparisonResult.OneSideOnly, result2);
        }

        [Fact]
        public static void GetNullStatus_WhenObjectVsNull_ReturnsBothNullStatus()
        {
            // Arrange
            object? nonObj1 = null;
            object? nonObj2 = null;

            // Act
            var result = NullHelper.NullComparison(nonObj1, nonObj2);

            // Assert
            Assert.Equal(NullComparisonResult.BothNull, result);
        }
    }
}
