using Rezaee.Iranseda.Configurations;
using static Rezaee.Iranseda.UnitTests.Helpers.UriHelper;

namespace Rezaee.Iranseda.UnitTests
{
    public class EpisodeTests
    {
        #region Tests for Equals()
        [Fact]
        public static void Equals_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var result = Episode.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Episode notNull = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode? @null = null;

            // Act
            var result = Episode.Equals(notNull, @null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var result = Episode.Equals(one, notSameIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_SamePartitions_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var result = Episode.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentPartitions_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSamePartitions =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var result = Episode.Equals(one, notSamePartitions);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_TrueCheckIdentityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration trueCheckIdentity = new() { CheckIdentity = true };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var result = Episode.Equals(one, sameOne, config: trueCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckIdentityConfig_DifferentIdentities_ReturnsFalse()
        {
            // Arrange
            EpisodesEqualityConfiguration trueCheckIdentity = new() { CheckIdentity = true };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var result = Episode.Equals(one, notSameIdentity, config: trueCheckIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckIdentityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration falseCheckIdentity = new() { CheckIdentity = false };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var result = Episode.Equals(one, notSameIdentity, config: falseCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckIdentityConfig_DifferentIdentities_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration falseCheckIdentity = new() { CheckIdentity = false };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var result = Episode.Equals(one, notSameIdentity, config: falseCheckIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_SameParent_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration trueCheckParents = new() { CheckParents = true };

            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "bar")
                };

            Episode sameParent =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "bar")
                };

            // Act
            var result = Episode.Equals(one, sameParent, config: trueCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_DifferentParents_ReturnsFalse()
        {
            // Arrange
            EpisodesEqualityConfiguration trueCheckParents = new() { CheckParents = true };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "bar")
                };
            Episode notSameParents =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "baz")
                };

            // Act
            var result = Episode.Equals(one, notSameParents, config: trueCheckParents);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_SameParent_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration falseCheckParents = new() { CheckParents = false };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "bar")
                };
            Episode sameParent =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "bar")
                };

            // Act
            var result = Episode.Equals(one, sameParent, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_DifferentParents_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration falseCheckParents = new() { CheckParents = false };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "01"), name: "bar")
                };
            Episode notSameParents =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Programme = new(url: MakeProgrammeUrl(ch: "01", m: "02"), name: "baz")
                };

            // Act
            var result = Episode.Equals(one, notSameParents, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckPartitionsIdentityConfig_SamePartition_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration trueCheckPartitionsIdentity = new() { PartitionsConfig = new() { CheckIdentity = true } };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var result = Episode.Equals(one, sameOne, config: trueCheckPartitionsIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckPartitionsIdentityConfig_DifferentPartitions_ReturnsFalse()
        {
            // Arrange
            EpisodesEqualityConfiguration trueCheckPartitionsIdentity = new() { PartitionsConfig = new() { CheckIdentity = true } };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSameChilds =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var result = Episode.Equals(one, notSameChilds, config: trueCheckPartitionsIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckPartitionsIdentityConfig_SamePartition_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration falseCheckPartitionsIdentity = new() { PartitionsConfig = new() { CheckIdentity = false } };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var result = Episode.Equals(one, sameOne, config: falseCheckPartitionsIdentity);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckPartitionsIdentityConfig_DifferentPartitions_ReturnsTrue()
        {
            // Arrange
            EpisodesEqualityConfiguration falseCheckPartitionsIdentity = new() { PartitionsConfig = new() { CheckIdentity = false } };
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { }),
                        new(time: "08:30", mirrors: new() { })
                    }
                };

            Episode notSameChilds =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { }),
                        new(time: "09:30", mirrors: new() { })
                    }
                };

            // Act
            var result = Episode.Equals(one, notSameChilds, config: falseCheckPartitionsIdentity);

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Merge()
        [Fact]
        public static void Merge_CompareMergeResultOfTwoSame_Equals()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act && Assert
            Assert.Equal(one, Episode.Merge(one, sameOne));
        }

        [Fact]
        public static void Merge_DoesContainsAllPartitionsFromBoth_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSamePartitions =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            Episode mergeActual =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") }),
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var mergePredict = Episode.Merge(one, notSamePartitions);
            var result = mergePredict == mergeActual;

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for override(s)
        #region Tests for operator ==
        [Fact]
        public static void EqualsToOperator_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_OneNull_ReturnsFalse()
        {
            // Arrange
            Episode notNull = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode? @null = null;

            // Act
            var result = notNull == @null!;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var result = one == notSameIdentity;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_SamePartitions_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_DifferentPartitions_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSamePartitions =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var result = one == notSamePartitions;

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Tests for operator !=
        [Fact]
        public static void NotEqualsToOperator_SameIdentity_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_OneNull_ReturnsTrue()
        {
            // Arrange
            Episode notNull = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode? @null = null;

            // Act
            var result = notNull != @null!;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DifferentIdentity_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var result = one != notSameIdentity;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualsToOperator_SamePartitions_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualsToOperator_DifferentPartitions_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSamePartitions =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var result = one != notSamePartitions;

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Equals()
        [Fact]
        public static void OverridedEquals_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_OneNull_ReturnsFalse()
        {
            // Arrange
            Episode notNull = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode? @null = null;

            // Act
            var result = notNull.Equals(@null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DifferentIdentity_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var result = one.Equals(notSameIdentity);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_SamePartitions_ReturnsTrue()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_DifferentPartitions_ReturnsFalse()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSamePartitions =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var result = one.Equals(notSamePartitions);

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Tests for GetHashCode()
        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameIdentities_Equals()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);

            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentIdentities_NotEquals()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today);
            Episode notSameIdentity =
                new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1));

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameIdentity = notSameIdentity.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameIdentity);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfSamePartitions_Equals()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode sameOne =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };

            // Act
            var hashOne = one.GetHashCode();
            var hashSameOne = sameOne.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashSameOne);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentPartitions_NotEquals()
        {
            // Arrange
            Episode one =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "08:00", mirrors: new() { new Uri("https://example.org/1"), new Uri("https://example.org/2") }),
                        new(time: "08:30", mirrors: new() { new Uri("https://example.org/3"), new Uri("https://example.org/4") })
                    }
                };
            Episode notSameChilds =
                new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
                {
                    Partitions = new()
                    {
                        new(time: "09:00", mirrors: new() { new Uri("https://example.org/5"), new Uri("https://example.org/6") }),
                        new(time: "09:30", mirrors: new() { new Uri("https://example.org/7"), new Uri("https://example.org/8") })
                    }
                };

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameChilds = notSameChilds.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameChilds);
        }
        #endregion
        #endregion
    }
}
