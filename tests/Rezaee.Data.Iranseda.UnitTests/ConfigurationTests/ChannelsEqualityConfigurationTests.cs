using Rezaee.Data.Iranseda.Configurations;

namespace Rezaee.Data.Iranseda.UnitTests.ConfigurationTests
{
    public class ChannelsEqualityConfigurationTests
    {
        [Fact]
        public static void EnsureDefaultProperties()
        {
            // Arrange
            ChannelsEqualityConfiguration config = new();

            // Act && Assert
            Assert.True(config.CheckIdentity);
            Assert.False(config.CheckParents);
            Assert.True(config.CheckProgrammes);
        }
    }
}
