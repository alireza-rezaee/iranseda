namespace Rezaee.Data.Iranseda.Configurations
{
    /// <summary>
    /// Get or set equality configuration for <see cref="Episode"/>s comparison
    /// </summary>
    public class EpisodesEqualityConfiguration : IEqualityConfiguration
    {
        /// <summary>true</summary>
        private const bool DefaultCheckIdentity = true;

        /// <summary>false</summary>
        private const bool DefaultCheckParents = false;

        /// <summary>true</summary>
        private const bool DefaultCheckPartitions = true;

        /// <summary>
        /// Check whether the <see cref="Episode.Identity"/>(-ies) are equal.
        /// The default value is <inheritdoc cref="DefaultCheckIdentity"/>.
        /// </summary>
        public bool CheckIdentity { get; set; } = DefaultCheckIdentity;

        /// <summary>
        /// Check whether the <see cref="Episode.Programme"/>s are equal.
        /// The default value is <inheritdoc cref="DefaultCheckParents"/>.
        /// </summary>
        public bool CheckParents { get; set; } = DefaultCheckParents;

        /// <summary>
        /// Check whether the <see cref="Episode.Partitions"/> are equal.
        /// The default value is <inheritdoc cref="DefaultCheckEpisodes"/>.
        /// </summary>
        public bool CheckPartitions { get; set; } = DefaultCheckPartitions;

        /// <inheritdoc cref="PartitionsEqualityConfiguration"/>
        public PartitionsEqualityConfiguration PartitionsConfig { get; set; } = new PartitionsEqualityConfiguration();
    }
}
