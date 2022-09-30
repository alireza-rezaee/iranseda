using Rezaee.Iranseda.Configurations;

namespace Rezaee.Iranseda.UnitTests.ConfigurationTests
{
    public class CataloguesEqualityConfigurationTests
    {
        [Fact]
        public static void EnsureDefaultProperties()
        {
            // Arrange
            CataloguesEqualityConfiguration config = new();

            // Act && Assert
            Assert.True(config.CheckChannels);
        }
    }
}
