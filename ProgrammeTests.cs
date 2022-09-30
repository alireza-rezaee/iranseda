using Rezaee.Iranseda.Configurations;
using static Rezaee.Iranseda.UnitTests.Helpers.UriHelper;

namespace Rezaee.Iranseda.UnitTests
{
    public class ProgrammeTests
    {
        #region Tests for Equals()
        [Fact]
        public static void Equals_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

            // Act
            var result = Programme.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Programme notNull =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme? @null = null;

            // Act
            var result = Programme.Equals(notNull, @null);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");

            // Act
            var result = Programme.Equals(one, notSameIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_SameEpisodes_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });

            // Act
            var result = Programme.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentEpisodes_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });

            // Act
            var result = Programme.Equals(one, notSameChilds);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_TrueCheckIdentityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration trueCheckIdentity = new() { CheckIdentity = true };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

            // Act
            var result = Programme.Equals(one, sameOne, config: trueCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckIdentityConfig_DifferentIdentities_ReturnsFalse()
        {
            // Arrange
            ProgrammesEqualityConfiguration trueCheckIdentity = new() { CheckIdentity = true };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");

            // Act
            var result = Programme.Equals(one, notSameIdentity, config: trueCheckIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckIdentityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration falseCheckIdentity = new() { CheckIdentity = false };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

            // Act
            var result = Programme.Equals(one, sameOne, config: falseCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckIdentityConfig_DifferentIdentities_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration falseCheckIdentity = new() { CheckIdentity = false };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");

            // Act
            var result = Programme.Equals(one, notSameIdentity, config: falseCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_SameParent_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration trueCheckParents = new() { CheckParents = true };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("01"), name: "bar")
                };
            Programme sameParent =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("01"), name: "bar")
                };

            // Act
            var result = Programme.Equals(one, sameParent, config: trueCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_DifferentParents_ReturnsFalse()
        {
            // Arrange
            ProgrammesEqualityConfiguration trueCheckParents = new() { CheckParents = true };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("01"), name: "bar")
                };
            Programme notSameParents =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("02"), name: "baz")
                };

            // Act
            var result = Programme.Equals(one, notSameParents, config: trueCheckParents);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_SameParent_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration falseCheckParents = new() { CheckParents = false };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("01"), name: "bar")
                };
            Programme sameParent =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("01"), name: "bar")
                };

            // Act
            var result = Programme.Equals(one, sameParent, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_DifferentParents_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration falseCheckParents = new() { CheckParents = false };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("01"), name: "bar")
                };
            Programme notSameParents =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo")
                {
                    Channel = new Channel(url: MakeChannelUrl("02"), name: "baz")
                };

            // Act
            var result = Programme.Equals(one, notSameParents, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckEpisodesIdentityConfig_SameEpisode_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration trueCheckEpisodesIdentity = new() { EpisodesConfig = new() { CheckIdentity = true } };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });

            // Act
            var result = Programme.Equals(one, sameOne, config: trueCheckEpisodesIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckEpisodesIdentityConfig_DifferentEpisodes_ReturnsFalse()
        {
            // Arrange
            ProgrammesEqualityConfiguration trueCheckEpisodesIdentity = new() { EpisodesConfig = new() { CheckIdentity = true } };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "03"), name: "qux", date: DateTime.Today.AddDays(2)),
                    new(url: MakeEpisodeUrl(ch: "01", e: "04"), name: "quux", date: DateTime.Today.AddDays(3))
                });

            // Act
            var result = Programme.Equals(one, notSameChilds, config: trueCheckEpisodesIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckEpisodesIdentityConfig_SameEpisode_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration falseCheckEpisodesIdentity = new() { EpisodesConfig = new() { CheckIdentity = false } };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });

            // Act
            var result = Programme.Equals(one, sameOne, config: falseCheckEpisodesIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckEpisodesIdentityConfig_DifferentEpisodes_ReturnsTrue()
        {
            // Arrange
            ProgrammesEqualityConfiguration falseCheckEpisodesIdentity = new() { EpisodesConfig = new() { CheckIdentity = false } };
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "03"), name: "qux", date: DateTime.Today.AddDays(2)),
                    new(url: MakeEpisodeUrl(ch: "01", e: "04"), name: "quux", date: DateTime.Today.AddDays(3))
                });

            // Act
            var result = Programme.Equals(one, notSameChilds, config: falseCheckEpisodesIdentity);

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Merge()
        [Fact]
        public static void Merge_CompareMergeResultOfTwoSame_Equals()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });

            // Act && Assert
            Assert.Equal(one, Programme.Merge(one, sameOne));
        }

        [Fact]
        public static void Merge_DifferentIdentities_ThrowsInvalidOperationException()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "baz", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "qux", date: DateTime.Today.AddDays(1))
                });
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "baz", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "qux", date: DateTime.Today.AddDays(1))
                });

            // Act
            void action() => Programme.Merge(one, notSameIdentity);
            // Assert
            Assert.Throws<InvalidOperationException>(action);
        }

        [Fact]
        public static void Merge_DoesContainsAllEpisodesFromBoth_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "03"), name: "qux", date: DateTime.Today.AddDays(2)),
                    new(url: MakeEpisodeUrl(ch: "01", e: "04"), name: "quux", date: DateTime.Today.AddDays(3))
                });
            Programme mergeActual =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today),
                    new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1)),
                    new(url: MakeEpisodeUrl(ch: "01", e: "03"), name: "qux", date: DateTime.Today.AddDays(2)),
                    new(url: MakeEpisodeUrl(ch: "01", e: "04"), name: "quux", date: DateTime.Today.AddDays(3))
                });

            // Act
            var mergePredict = Programme.Merge(one, notSameChilds);
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
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Programme notNull =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme? @null = null;

            // Act
            var result = notNull == @null;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");

            // Act
            var result = one == notSameIdentity;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_SameEpisodes_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DefaultEqualityConfig_DifferentEpisodes_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
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
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_OneNull_ReturnsTrue()
        {
            // Arrange
            Programme notNull =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme? @null = null;

            // Act
            var result = notNull != @null;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_DifferentIdentity_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");

            // Act
            var result = one != notSameIdentity;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_SameEpisodes_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DefaultEqualityConfig_DifferentEpisodes_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
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
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Programme notNull =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme? @null = null;

            // Act
            var result = notNull.Equals(@null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");

            // Act
            var result = one.Equals(notSameIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_SameEpisodes_ReturnsTrue()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DefaultEqualityConfig_DifferentEpisodes_ReturnsFalse()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
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
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");

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
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo");
            Programme notSameIdentity =
                new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "bar");


            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameIdentity = notSameIdentity.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameIdentity);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameEpisodes_Equals()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme sameOne =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });


            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentEpisodes_NotEquals()
        {
            // Arrange
            Programme one =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "bar", date: DateTime.Today)
                });
            Programme notSameChilds =
                new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "foo", episodes: new()
                {
                    new Episode(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "baz", date: DateTime.Today.AddDays(1))
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
