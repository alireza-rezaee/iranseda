namespace Rezaee.Data.Iranseda.Configurations
{
    /// <summary>
    /// Get or set equality configuration for <see cref="Programme"/>s comparison
    /// </summary>
    public class ProgrammesEqualityConfiguration : IEqualityConfiguration
    {
        /// <summary>true</summary>
        private const bool DefaultCheckIdentity = true;

        /// <summary>false</summary>
        private const bool DefaultCheckParents = false;

        /// <summary>true</summary>
        private const bool DefaultCheckEpisodes = true;

        /// <summary>
        /// Check whether the <see cref="Programme.Identity"/>(-ies) are equal.
        /// The default value is <inheritdoc cref="DefaultCheckIdentity"/>.
        /// </summary>
        public bool CheckIdentity { get; set; } = DefaultCheckIdentity;

        /// <summary>
        /// Check whether the <see cref="Programme.Channel"/>s are equal.
        /// The default value is <inheritdoc cref="DefaultCheckParents"/>.
        /// </summary>
        public bool CheckParents { get; set; } = DefaultCheckParents;

        /// <summary>
        /// Check whether the <see cref="Programme.Episodes"/> are equal.
        /// The default value is <inheritdoc cref="DefaultCheckEpisodes"/>.
        /// </summary>
        public bool CheckEpisodes { get; set; } = DefaultCheckEpisodes;

        /// <inheritdoc cref="EpisodesEqualityConfiguration"/>
        public EpisodesEqualityConfiguration EpisodesConfig { get; set; } = new EpisodesEqualityConfiguration();

    }
}
