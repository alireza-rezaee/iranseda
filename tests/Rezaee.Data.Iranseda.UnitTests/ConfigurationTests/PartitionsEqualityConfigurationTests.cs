using Rezaee.Data.Iranseda.Configurations;

namespace Rezaee.Data.Iranseda.UnitTests.ConfigurationTests
{
    public class PartitionsEqualityConfigurationTests
    {
        [Fact]
        public static void EnsureDefaultProperties()
        {
            // Arrange
            PartitionsEqualityConfiguration config = new();

            // Act && Assert
            Assert.True(config.CheckIdentity);
            Assert.False(config.CheckParents);
            Assert.True(config.CheckMirrors);
            Assert.True(config.CheckDownloads);
        }
    }
}
