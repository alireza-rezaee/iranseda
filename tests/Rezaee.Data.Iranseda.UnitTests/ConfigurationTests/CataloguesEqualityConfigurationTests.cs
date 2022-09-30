using Rezaee.Data.Iranseda.Configurations;

namespace Rezaee.Data.Iranseda.UnitTests.ConfigurationTests
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
