using Rezaee.Iranseda.Configurations;

namespace Rezaee.Iranseda.UnitTests.ConfigurationTests
{
    public class ProgrammesEqualityConfigurationTests
    {
        [Fact]
        public static void EnsureDefaultProperties()
        {
            // Arrange
            ProgrammesEqualityConfiguration config = new();

            // Act && Assert
            Assert.True(config.CheckIdentity);
            Assert.False(config.CheckParents);
            Assert.True(config.CheckEpisodes);
        }
    }
}
