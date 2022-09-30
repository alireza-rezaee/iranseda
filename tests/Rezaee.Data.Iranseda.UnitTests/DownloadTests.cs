namespace Rezaee.Data.Iranseda.UnitTests
{
    public class DownloadTests
    {
        #region Tests for operator ==
        [Fact]
        public static void EqualToOperator_AreSameDownloadInfosEqual_ReturnsTrue()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download sameOne = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualToOperator_AreDifferentPathDownloadInfosEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSamePath = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-2");

            // Act
            var result = one == notSamePath;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualToOperator_AreDifferentUrlDownloadInfosEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameUrl = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-1");

            // Act
            var result = one == notSameUrl;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualToOperator_AreDifferentDownloadInfosEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameOne = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-2");

            // Act
            var result = one == notSameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualToOperator_AreDownloadInfoAndNullEqual_ReturnsFalse()
        {
            // Arrange
            Download notNull = new(url: new("https://example.org/one"), path: @"C:\\path-to-file");
            Download? @null = null;

            // Act
            var result1 = notNull == @null;
            var result2 = @null == notNull;

            // Assert
            Assert.False(result1);
            Assert.False(result2);
        }
        #endregion

        #region Tests for operator !=
        [Fact]
        public static void NotEqualToOperator_AreSameDownloadInfosNotEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download sameOne = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualToOperator_AreDifferentPathDownloadInfosNotEqual_ReturnsTrue()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSamePath = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-2");

            // Act
            var result = one != notSamePath;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_AreDifferentUrlDownloadInfosNotEqual_ReturnsTrue()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameUrl = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-1");

            // Act
            var result = one != notSameUrl;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_AreDifferentDownloadInfosNotEqual_ReturnsTrue()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameOne = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-2");

            // Act
            var result = one != notSameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_AreDownloadInfoAndNullNotEqual_ReturnsTrue()
        {
            // Arrange
            Download notNull = new(url: new("https://example.org/one"), path: @"C:\\path-to-file");
            Download? @null = null;

            // Act
            var result1 = notNull != @null;
            var result2 = @null != notNull;

            // Assert
            Assert.True(result1);
            Assert.True(result2);
        }
        #endregion

        #region Tests for Equals()
        [Fact]
        public static void Equals_AreSameDownloadInfosEqual_ReturnsTrue()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download sameOne = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_AreDifferentPathDownloadInfosEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSamePath = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-2");

            // Act
            var result = one.Equals(notSamePath);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_AreDifferentUrlDownloadInfosEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameUrl = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-1");

            // Act
            var result = one.Equals(notSameUrl);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_AreDifferentDownloadInfosEqual_ReturnsFalse()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameOne = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-2");

            // Act
            var result = one.Equals(notSameOne);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_AreDownloadInfoAndNullEqual_ReturnsFalse()
        {
            // Arrange
            Download notNull = new(url: new("https://example.org/one"), path: @"C:\\path-to-file");
            Download? @null = null;

            // Act
            var result = notNull.Equals(@null!);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Tests for GetHashCode()
        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameDownloadInfosHashCode_Equals()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file");
            Download sameOne = new(url: new("https://example.org/one"), path: @"C:\\path-to-file");

            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentPathDownloadInfosHashCode_NotEquals()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSamePath = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-2");

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSamePath = notSamePath.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSamePath);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentUrlDownloadInfosHashCode_NotEquals()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file");
            Download notSameUrl = new(url: new("https://example.org/other"), path: @"C:\\path-to-file");

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameUrl = notSameUrl.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameUrl);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentDownloadInfosHashCode_NotEquals()
        {
            // Arrange
            Download one = new(url: new("https://example.org/one"), path: @"C:\\path-to-file-1");
            Download notSameOne = new(url: new("https://example.org/other"), path: @"C:\\path-to-file-2");

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameOne = notSameOne.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameOne);
        }
        #endregion
    }
}
