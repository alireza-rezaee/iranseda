namespace Rezaee.Iranseda.Configurations
{
    /// <summary>
    /// Get or set equality configuration for <see cref="Catalogue"/>s comparison
    /// </summary>
    public class CataloguesEqualityConfiguration : IEqualityConfiguration
    {
        /// <summary>true</summary>
        private const bool DefaultCheckChannels = true;

        /// <summary>
        /// Check whether the <see cref="Catalogue.Channels"/> are equal.
        /// The default value is <inheritdoc cref="DefaultCheckChannels"/>.
        /// </summary>
        public bool CheckChannels { get; set; } = DefaultCheckChannels;

        /// <inheritdoc cref="ChannelsEqualityConfiguration"/>
        public ChannelsEqualityConfiguration ChannelsConfig { get; set; } = new ChannelsEqualityConfiguration();
    }
}
