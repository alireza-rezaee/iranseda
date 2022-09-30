using Rezaee.Iranseda.Configurations;
using static Rezaee.Iranseda.UnitTests.Helpers.UriHelper;

namespace Rezaee.Iranseda.UnitTests
{
    public class PartitionTests
    {
        #region Tests for Equals()
        [Fact]
        public static void Equals_DefaultEqualityConfig_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            // Act
            var result = Partition.Equals(one, sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_OneNull_ReturnsFalse()
        {
            // Arrange
            Partition? @null = null;
            Partition notNull = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            // Act
            var result = Partition.Equals(notNull, @null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_DefaultEqualityConfig_DifferentIdentities_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameTime = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "00:00");

            // Act
            var result = Partition.Equals(one, notSameTime);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_TrueCheckMirrorsConfig_SameMirrors_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration trueCheckMirrors = new() { CheckMirrors = true };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
            }, time: "12:00");

            // Act
            var result = Partition.Equals(one, sameMirrors, config: trueCheckMirrors);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckMirrorsConfig_DifferentMirrors_ReturnsFalse()
        {
            // Arrange
            PartitionsEqualityConfiguration trueCheckMirrors = new() { CheckMirrors = true };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/one-more")
            }, time: "12:00");

            // Act
            var result = Partition.Equals(one, notSameMirrors, config: trueCheckMirrors);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckMirrorsConfig_SameMirrors_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration falseCheckMirrors = new() { CheckMirrors = false };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
            }, time: "12:00");

            // Act
            var result = Partition.Equals(one, sameMirrors, config: falseCheckMirrors);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckMirrorsConfig_DifferentMirrors_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration falseCheckMirrors = new() { CheckMirrors = false };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/one-more")
            }, time: "12:00");

            // Act
            var result = Partition.Equals(one, notSameMirrors, config: falseCheckMirrors);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckDownloadsConfig_SameDownloads_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration trueCheckDownloads = new() { CheckDownloads = true };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/same"), path: @"C:\\same")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/same"), path: @"C:\\same")
            };

            // Act
            var result = Partition.Equals(one, notSameDownload, config: trueCheckDownloads);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckDownloadsConfig_DifferentDownloads_ReturnsFalse()
        {
            // Arrange
            PartitionsEqualityConfiguration trueCheckDownloads = new() { CheckDownloads = true };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/1"), path: @"C:\\path-to-file-1")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/2"), path: @"C:\\path-to-file-2")
            };

            // Act
            var result = Partition.Equals(one, notSameDownload, config: trueCheckDownloads);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckDownloadsConfig_SameDownloads_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration falseCheckDownloads = new() { CheckDownloads = false };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/same"), path: @"C:\\same")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/same"), path: @"C:\\same")
            };

            // Act
            var result = Partition.Equals(one, notSameDownload, config: falseCheckDownloads);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckDownloadsConfig_DifferentDownloads_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration falseCheckDownloads = new() { CheckDownloads = false };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/1"), path: @"C:\\path-to-file-1")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/2"), path: @"C:\\path-to-file-2")
            };

            // Act
            var result = Partition.Equals(one, notSameDownload, config: falseCheckDownloads);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_SameParents_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration trueCheckParents = new() { CheckParents = true };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition sameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            // Act
            var result = Partition.Equals(one, sameParrents, config: trueCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_TrueCheckParentsConfig_DifferentParents_ReturnsFalse()
        {
            // Arrange
            PartitionsEqualityConfiguration trueCheckParents = new() { CheckParents = true };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition notSameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1))
            };

            // Act
            var result = Partition.Equals(one, notSameParrents, config: trueCheckParents);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_SameParents_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration falseCheckParents = new() { CheckParents = false };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition sameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            // Act
            var result = Partition.Equals(one, sameParrents, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void Equals_FalseCheckParentsConfig_DifferentParents_ReturnsTrue()
        {
            // Arrange
            PartitionsEqualityConfiguration falseCheckParents = new() { CheckParents = false };

            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition notSameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1))
            };

            // Act
            var result = Partition.Equals(one, notSameParrents, config: falseCheckParents);

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for Merge()
        [Fact]
        public static void Merge_CompareMergeResultOfTwoSame_Equals()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            // Act && Assert
            Assert.Equal(one, Partition.Merge(one, sameOne));
        }

        [Fact]
        public static void Merge_DifferentTime_ThrowsInvalidOperationException()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameTime = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "00:00");

            // Act
            void action1() => Partition.Merge(one, notSameTime);
            void action2() => Partition.Merge(one, notSameTime);
            void action3() => Partition.Merge(one, notSameTime);

            // Assert
            Assert.Throws<InvalidOperationException>(action1);
            Assert.Throws<InvalidOperationException>(action2);
            Assert.Throws<InvalidOperationException>(action3);
        }

        [Fact]
        public static void Merge_DoesContainsAllMirrorsFromBoth_ReturnsTrue()
        {
            // Arrange
            Partition newer = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/newer-only")
            }, time: "12:00");

            Partition older = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/old-only")
            }, time: "12:00");

            Partition mergeActual = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/newer-only"),
                new Uri("https://example.org/old-only")
            }, time: "12:00");

            // Act
            var mergePredict = Partition.Merge(newer, older);
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
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            // Act
            var result = one == sameOne;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void EqualsToOperator_OneNull_ReturnsFalse()
        {
            // Arrange
            Partition notNull = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");
            Partition? @null = null;

            // Act
            var result = notNull == @null!;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DifferentIdentities_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameTime = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "00:00");

            // Act
            var result = one == notSameTime;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DifferentMirrors_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/other-one")
            }, time: "12:00");

            // Act
            var result = one == notSameMirrors;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DifferentDownloads_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/1"), path: @"C:\\path-to-file-1")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/2"), path: @"C:\\path-to-file-2")
            };

            // Act
            var result = one == notSameDownload;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void EqualsToOperator_DifferentParents_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition notSameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01" , "02"), name: "bar", date: DateTime.Today)
            };

            // Act
            var result = one == notSameParrents;

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for operator !=
        [Fact]
        public static void NotEqualToOperator_SameIdentity_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            // Act
            var result = one != sameOne;

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void NotEqualToOperator_OneNull_ReturnsTrue()
        {
            // Arrange
            Partition notNull = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");
            Partition? @null = null;

            // Act
            var result = notNull != @null!;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_DifferentIdentities_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameTime = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "00:00");

            // Act
            var result = one != notSameTime;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_DifferentMirrors_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/other-one")
            }, time: "12:00");

            // Act
            var result = one != notSameMirrors;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_DifferentDownloads_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/1"), path: @"C:\\path-to-file-1")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/2"), path: @"C:\\path-to-file-2")
            };

            // Act
            var result = one != notSameDownload;

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void NotEqualToOperator_DifferentParents_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition notSameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1))
            };

            // Act
            var result = one != notSameParrents;

            // Assert
            Assert.False(result);
        }
        #endregion

        #region Tests for Equals()
        [Fact]
        public static void OverridedEquals_SameIdentity_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            // Act
            var result = one.Equals(sameOne);

            // Assert
            Assert.True(result);
        }

        [Fact]
        public static void OverridedEquals_OneNull_ReturnsFalse()
        {
            // Arrange
            Partition notNullPartition = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");
            object? @null = null;

            // Act
            var result = notNullPartition.Equals(@null!);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DifferentIdentities_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition otherOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "00:00");

            // Act
            var result = one.Equals(otherOne);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DifferentMirrors_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/one-more")
            }, time: "12:00");

            // Act
            var result = one.Equals(notSameMirrors);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DifferentDownloads_ReturnsFalse()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/1"), path: @"C:\\path-to-file-1")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/2"), path: @"C:\\path-to-file-2")
            };

            // Act
            var result = one.Equals(notSameDownload);

            // Assert
            Assert.False(result);
        }

        [Fact]
        public static void OverridedEquals_DifferentParents_ReturnsTrue()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition notSameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1))
            };

            // Act
            var result = one.Equals(notSameParrents);

            // Assert
            Assert.True(result);
        }
        #endregion

        #region Tests for GetHashCode()
        [Fact]
        public static void GetHashCode_CompareHashCodeOfSameIdentities_Equals()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition sameOne = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

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
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameTime = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "00:00");

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameTime = notSameTime.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameTime);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentMirrors_NotEquals()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00");

            Partition notSameMirrors = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2"),
                new Uri("https://example.org/one-more")
            }, time: "12:00");

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameMirrors = notSameMirrors.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameMirrors);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentDownloads_NotEquals()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/1"), path: @"C:\\path-to-file-1")
            };

            Partition notSameDownload = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Download = new(url: new("https://example.org/2"), path: @"C:\\path-to-file-2")
            };

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameDownload = notSameDownload.GetHashCode();

            // Assert
            Assert.NotEqual(hashOne, hashNotSameDownload);
        }

        [Fact]
        public static void GetHashCode_CompareHashCodeOfDifferentParents_Equals()
        {
            // Arrange
            Partition one = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "01"), name: "foo", date: DateTime.Today)
            };

            Partition notSameParrents = new(mirrors: new()
            {
                new Uri("https://example.org/1"),
                new Uri("https://example.org/2")
            }, time: "12:00")
            {
                Episode = new(url: MakeEpisodeUrl(ch: "01", e: "02"), name: "bar", date: DateTime.Today.AddDays(1))
            };

            // Act
            var hashOne = one.GetHashCode();
            var hashNotSameParrents = notSameParrents.GetHashCode();

            // Assert
            Assert.Equal(hashOne, hashNotSameParrents);
        }
        #endregion
        #endregion
    }
}
