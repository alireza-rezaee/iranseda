using Rezaee.Data.Iranseda.Extensions;

namespace Rezaee.Data.Iranseda.UnitTests.ExtensionsTests
{
    public class ListExtensionsTests
    {
        [Fact]
        public static void GetHashCodeByItems_CompareTwoListWhichContainSameItems_Equals()
        {
            // Arrange
            List<dynamic> one = new() { 123, "123", true, '\u0123', (byte)123, DateTime.Today };
            List<dynamic> sameOne = new() { 123, "123", true, '\u0123', (byte)123, DateTime.Today };

            // Act
            var hashOne = one.GetOrderIndependentHashCode();
            var hashSameOne = sameOne.GetOrderIndependentHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCodeByItems_CompareTwoListWhichContainSameItemsButInDifferentOrder_Equals()
        {
            // Arrange
            List<dynamic> one = new() { 123, "123", true, '\u0123', (byte)123, DateTime.Today };
            List<dynamic> rearrangedOne = new() { DateTime.Today, 123, "123", true, '\u0123', (byte)123 };

            // Act
            var hashOne = one.GetOrderIndependentHashCode();
            var hashRearrangedOne = rearrangedOne.GetOrderIndependentHashCode();

            // Assert
            Assert.Equal(hashOne, hashRearrangedOne);
        }

        [Fact]
        public static void GetHashCodeByItems_CompareTwoListWhichContainSameItemsButOneSideHasOneMoreNullItem_NotEquals()
        {
            // Arrange
            List<dynamic?> one = new() { 123, "123", true, '\u0123', (byte)123, DateTime.Today };
            List<dynamic?> oneNullMore = new() { 123, "123", true, '\u0123', (byte)123, DateTime.Today, null };

            // Act
            var hashOne = one.GetOrderIndependentHashCode();
            var hashOneNullMore = oneNullMore.GetOrderIndependentHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashOneNullMore);
        }

        [Fact]
        public static void GetHashCodeByItems_CompareTwoListWhichContainDifferentItems_NotEquals()
        {
            // Arrange
            List<dynamic?> one = new() { 123, "123", true, '\u0123', (byte)123, DateTime.Today, null };
            List<dynamic> otherOne = new() { 456, "456", false, '\u0456', (short)456 };

            // Act
            var hashOne = one.GetOrderIndependentHashCode();
            var hashOtherOne = otherOne.GetOrderIndependentHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashOtherOne);
        }

        [Fact]
        public static void GetHashCodeByItems_CompareTwoEmptyLists_Equals()
        {
            // Arrange
            List<dynamic> one = new() { };
            List<dynamic> sameOne = new() { };

            // Act
            var hashOne = one.GetOrderIndependentHashCode();
            var hashSameOne = sameOne.GetOrderIndependentHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCodeByItems_CompareOneEmptyListVsAnotherListWhichContainsANullItem_NotEquals()
        {
            // Arrange
            List<dynamic> one = new() { };
            List<dynamic?> oneNullMore = new() { null };

            // Act
            var hashOne = one.GetOrderIndependentHashCode();
            var hashOneNullMore = oneNullMore.GetOrderIndependentHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashOneNullMore);
        }
    }
}
