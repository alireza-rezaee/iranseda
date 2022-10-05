using Rezaee.Data.Iranseda.Configurations;
using static Rezaee.Data.Iranseda.UnitTests.Helpers.UriHelper;

namespace Rezaee.Data.Iranseda.UnitTests
{
    public class ChannelTests
    {
        #region Tests for Equals()
        [Fact]
        public static void Equals_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var result = Channel.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Channel notNull =
                new(id: "01", name: "foo");
            Channel? @null = null;

            // Act
            var result = Channel.Equals(notNull, @null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo");
            Channel notSameIdentity =
                new(id: "02", name: "bar");

            // Act
            var result = Channel.Equals(one, notSameIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_SameProgrammes_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });

            // Act
            var result = Channel.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentProgrammes_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

            // Act
            var result = Channel.Equals(one, notSameChilds);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_TrueCheckIdentityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration trueCheckIdentity = new() { CheckIdentity = true };
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var result = Channel.Equals(one, sameOne, config: trueCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckIdentityConfig_DifferentIdentities_ReturnsFalse()
        {
            // Arrange
            ChannelsEqualityConfiguration trueCheckIdentity = new() { CheckIdentity = true };
            Channel one =
                new(id: "01", name: "foo");
            Channel notSameIdentity =
                new(id: "02", name: "bar");

            // Act
            var result = Channel.Equals(one, notSameIdentity, config: trueCheckIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckIdentityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration falseCheckIdentity = new() { CheckIdentity = false };
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var result = Channel.Equals(one, sameOne, config: falseCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckIdentityConfig_DifferentIdentities_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration falseCheckIdentity = new() { CheckIdentity = false };
            Channel one =
                new(id: "01", name: "foo");
            Channel notSameIdentity =
                new(id: "02", name: "bar");

            // Act
            var result = Channel.Equals(one, notSameIdentity, config: falseCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_SameParent_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration trueCheckParents = new() { CheckParents = true };
            Channel one =
                new(id: "01", name: "foo") { Catalogue = new() };
            one.Catalogue.Channels = new() { one };
            Channel sameParent =
                new(id: "01", name: "foo") { Catalogue = new() };
            sameParent.Catalogue.Channels = new() { sameParent };

            // Act
            var result = Channel.Equals(one, sameParent, config: trueCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_DifferentParents_ReturnsFalse()
        {
            // Arrange
            ChannelsEqualityConfiguration trueCheckParents = new() { CheckParents = true };
            Channel one =
                new(id: "01", name: "foo") { Catalogue = new() };
            one.Catalogue.Channels = new() { one };

            Channel notSameParents =
                new(id: "01", name: "foo") { Catalogue = new() };
            notSameParents.Catalogue.Channels = new() { notSameParents };

            Channel extraChildToMakeParentsDifferent =
                new(id: "02", name: "bar");
            notSameParents.Catalogue.Channels.Add(extraChildToMakeParentsDifferent);

            // Act
            var result = Channel.Equals(one, notSameParents, config: trueCheckParents);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_SameParent_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration falseCheckParents = new() { CheckParents = false };
            Channel one =
                new(id: "01", name: "foo") { Catalogue = new() };
            one.Catalogue.Channels = new() { one };

            Channel sameParent =
                new(id: "01", name: "foo") { Catalogue = new() };
            one.Catalogue.Channels = new() { sameParent };

            // Act
            var result = Channel.Equals(one, sameParent, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_DifferentParents_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration falseCheckParents = new() { CheckParents = false };
            Channel one =
                new(id: "01", name: "foo") { Catalogue = new() };
            one.Catalogue.Channels = new() { one };

            Channel notSameParents =
                new(id: "01", name: "foo") { Catalogue = new() };
            notSameParents.Catalogue.Channels = new() { one, notSameParents };

            // Act
            var result = Channel.Equals(one, notSameParents, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckProgrammesIdentityConfig_SameProgramme_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration trueCheckProgrammesIdentity = new() { ProgrammesConfig = new() { CheckIdentity = true } };
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                });

            // Act
            var result = Channel.Equals(one, sameOne, config: trueCheckProgrammesIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckProgrammesIdentityConfig_DifferentProgrammes_ReturnsFalse()
        {
            // Arrange
            ChannelsEqualityConfiguration trueCheckProgrammesIdentity = new() { ProgrammesConfig = new() { CheckIdentity = true } };
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "03"), name: "qux"),
                    new(identity: (channelId: "01", programmeId: "04"), name: "quux"),
                });

            // Act
            var result = Channel.Equals(one, notSameChilds, config: trueCheckProgrammesIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckProgrammesIdentityConfig_SameProgramme_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration falseCheckProgrammesIdentity = new() { ProgrammesConfig = new() { CheckIdentity = false } };
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                });

            // Act
            var result = Channel.Equals(one, sameOne, config: falseCheckProgrammesIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckProgrammesIdentityConfig_DifferentProgrammes_ReturnsTrue()
        {
            // Arrange
            ChannelsEqualityConfiguration falseCheckProgrammesIdentity = new() { ProgrammesConfig = new() { CheckIdentity = false } };
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "03"), name: "qux"),
                    new(identity: (channelId: "01", programmeId: "04"), name: "quux"),
                });

            // Act
            var result = Channel.Equals(one, notSameChilds, config: falseCheckProgrammesIdentity);

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Merge()
        [Fact]
        public static void Merge_CompareMergeResultOfTwoSame_Equals()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

            // Act && Assert
            Assert.Equal(one, Channel.Merge(one, sameOne));
        }

        [Fact]
        public static void Merge_DifferentIdentities_ThrowsInvalidOperationException()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });
            Channel notSameIdentity =
                new(id: "02", name: "bar", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

            // Act
            void action() => Channel.Merge(one, notSameIdentity);

            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public static void Merge_DoesContainsAllProgrammesFromBoth_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "03"), name: "qux"),
                    new(identity: (channelId: "01", programmeId: "04"), name: "quux")
                });
            Channel mergeActual =
                new(id: "01", name: "foo", programmes: new()
                {
                    new(identity: (channelId: "01", programmeId: "01"), name: "bar"),
                    new(identity: (channelId: "01", programmeId: "02"), name: "baz"),
                    new(identity: (channelId: "01", programmeId: "03"), name: "qux"),
                    new(identity: (channelId: "01", programmeId: "04"), name: "quux")
                });

            // Act
            var mergePredict = Channel.Merge(one, notSameChilds);
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
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Channel notNull =
                new(id: "01", name: "foo");
            Channel? @null = null;

            // Act
            var result = notNull == @null;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo");
            Channel notSameIdentity =
                new(id: "02", name: "bar");

            // Act
            var result = one == notSameIdentity;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_SameProgrammes_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_DifferentProgrammes_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

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
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_OneNull_ReturnsTrue()
        {
            // Arrange
            Channel notNull =
                new(id: "01", name: "foo");
            Channel? @null = null;

            // Act
            var result = notNull != @null;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_DifferentIdentity_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo");
            Channel notSameIdentity =
                new(id: "02", name: "bar");

            // Act
            var result = one != notSameIdentity;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_SameProgrammes_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_DifferentProgrammes_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

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
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Channel notNull =
                new(id: "01", name: "foo");
            Channel? @null = null;

            // Act
            var result = notNull.Equals(@null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo");
            Channel notSameIdentity =
                new(id: "02", name: "bar");

            // Act
            var result = one.Equals(notSameIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_SameProgrammes_ReturnsTrue()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_DifferentProgrammes_ReturnsFalse()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

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
            Channel one =
                new(id: "01", name: "foo");
            Channel sameOne =
                new(id: "01", name: "foo");

            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentIdentity_NotEquals()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "channel-01");
            Channel notSameIdentity =
                new(id: "02", name: "channel-02");

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameIdentity = notSameIdentity.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameIdentity);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameProgrammes_Equals()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel sameOne =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });


            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentProgrammes_NotEquals()
        {
            // Arrange
            Channel one =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "01"), name: "bar")
                });
            Channel notSameChilds =
                new(id: "01", name: "foo", programmes: new()
                {
                    new Programme(identity: (channelId: "01", programmeId: "02"), name: "baz")
                });

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
