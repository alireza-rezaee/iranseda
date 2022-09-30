using Rezaee.Iranseda.Configurations;
using static Rezaee.Iranseda.UnitTests.Helpers.UriHelper;

namespace Rezaee.Iranseda.UnitTests
{
    public class CatalogueTests
    {
        #region Tests for Equals()
        [Fact]
        public static void Equals_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new();
            Catalogue sameOne = new();

            // Act
            var result = Catalogue.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Catalogue notNull = new();
            Catalogue? @null = null;

            // Act
            var result = Catalogue.Equals(notNull, @null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_SameChannels_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue sameOne = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);

            // Act
            var result = Catalogue.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentChannels_ReturnsFalse()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue notSameChilds = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new Channel(url: MakeChannelUrl(ch: "04"), name: "qux")
            }, lastModified: DateTime.Now);

            // Act
            var result = Catalogue.Equals(one, notSameChilds);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_TrueCheckChannelsIdentityConfig_SameChannel_ReturnsTrue()
        {
            // Arrange
            CataloguesEqualityConfiguration trueCheckChannelsIdentity = new() { ChannelsConfig = new() { CheckIdentity = true } };
            Catalogue one = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });
            Catalogue sameOne = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });

            // Act
            var result = Catalogue.Equals(one, sameOne, config: trueCheckChannelsIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckChannelsIdentityConfig_DifferentChannels_ReturnsFalse()
        {
            // Arrange
            CataloguesEqualityConfiguration trueCheckChannelsIdentity = new() { ChannelsConfig = new() { CheckIdentity = true } };
            Catalogue one = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });
            Catalogue notSameChilds = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new(url: MakeChannelUrl(ch: "04"), name: "qux")
            });

            // Act
            var result = Catalogue.Equals(one, notSameChilds, config: trueCheckChannelsIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckChannelsIdentityConfig_SameChannel_ReturnsTrue()
        {
            // Arrange
            CataloguesEqualityConfiguration falseCheckChannelsIdentity = new() { ChannelsConfig = new() { CheckIdentity = false } };
            Catalogue one = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });
            Catalogue sameOne = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });

            // Act
            var result = Catalogue.Equals(one, sameOne, config: falseCheckChannelsIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckChannelsIdentityConfig_DifferentChannels_ReturnsTrue()
        {
            // Arrange
            CataloguesEqualityConfiguration falseCheckChannelsIdentity = new() { ChannelsConfig = new() { CheckIdentity = false } };
            Catalogue one = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });
            Catalogue notSameChilds = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new(url: MakeChannelUrl(ch: "04"), name: "qux")
            });

            // Act
            var result = Catalogue.Equals(one, notSameChilds, config: falseCheckChannelsIdentity);

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Merge()
        [Fact]
        public static void Merge_CompareMergeResultOfTwoSame_Equals()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });
            Catalogue sameOne = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });

            // Act && Assert
            Assert.Equal(one, Catalogue.Merge(one, sameOne));
        }

        [Fact]
        public static void Merge_DoesContainsAllChannelsFromBoth_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar")
            });
            Catalogue notSameChilds = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new(url: MakeChannelUrl(ch: "04"), name: "qux")
            });
            Catalogue mergeActual = new(channels: new()
            {
                new(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new(url: MakeChannelUrl(ch: "02"), name: "bar"),
                new(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new(url: MakeChannelUrl(ch: "04"), name: "qux")
            });

            // Act
            var mergePredict = Catalogue.Merge(one, notSameChilds);
            var result = mergePredict == mergeActual;

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for override(s)
        #region Tests for operator ==
        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new();
            Catalogue sameOne = new();

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Catalogue notNull = new();
            Catalogue? @null = null;

            // Act
            var result = notNull == @null;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_SameChannels_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue sameOne = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_DifferentChannels_ReturnsFalse()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue notSameChilds = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new Channel(url: MakeChannelUrl(ch: "04"), name: "qux")
            }, lastModified: DateTime.Now);

            // Act
            var result = one == notSameChilds;

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Tests for operator !=
        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_SameIdentity_ReturnsFalse()
        {
            // Arrange
            Catalogue one = new();
            Catalogue sameOne = new();

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_OneNull_ReturnsTrue()
        {
            // Arrange
            Catalogue notNull = new();
            Catalogue? @null = null;

            // Act
            var result = notNull != @null;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_SameChannels_ReturnsFalse()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue sameOne = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_DifferentChannels_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue notSameChilds = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new Channel(url: MakeChannelUrl(ch: "04"), name: "qux")
            }, lastModified: DateTime.Now);

            // Act
            var result = one != notSameChilds;

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Equals()
        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new();
            Catalogue sameOne = new();

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Catalogue notNull = new();
            Catalogue? @null = null;

            // Act
            var result = notNull.Equals(@null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_SameChannels_ReturnsTrue()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue sameOne = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_DifferentChannels_ReturnsFalse()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue notSameChilds = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new Channel(url: MakeChannelUrl(ch: "04"), name: "qux")
            }, lastModified: DateTime.Now);

            // Act
            var result = one.Equals(notSameChilds);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Tests for GetHashCode()
        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameIdentity_Equals()
        {
            // Arrange
            Catalogue one = new();
            Catalogue sameOne = new();

            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameChannels_Equals()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue sameOne = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);


            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentChannels_NotEquals()
        {
            // Arrange
            Catalogue one = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "01"), name: "foo"),
                new Channel(url: MakeChannelUrl(ch: "02"), name: "bar")
            }, lastModified: DateTime.Now);
            Catalogue notSameChilds = new(channels: new()
            {
                new Channel(url: MakeChannelUrl(ch: "03"), name: "baz"),
                new Channel(url: MakeChannelUrl(ch: "04"), name: "qux")
            }, lastModified: DateTime.Now);

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSamePartitions = notSameChilds.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSamePartitions);
        }
        #endregion
        #endregion
    }
}
