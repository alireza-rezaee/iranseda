using Rezaee.Data.Iranseda.Configurations;

namespace Rezaee.Data.Iranseda.UnitTests.ConfigurationTests
{
    public class EpisodesEqualityConfigurationTests
    {
        [Fact]
        public static void EnsureDefaultProperties()
        {
            // Arrange
            EpisodesEqualityConfiguration config = new();

            // Act && Assert
            Assert.True(config.CheckIdentity);
            Assert.False(config.CheckParents);
            Assert.True(config.CheckPartitions);
        }
    }
}
