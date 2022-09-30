namespace Rezaee.Iranseda.Configurations
{
    /// <summary>
    /// Get or set equality configuration for <see cref="Partition"/>s comparison
    /// </summary>
    public class PartitionsEqualityConfiguration : IEqualityConfiguration
    {
        /// <summary>true</summary>
        private const bool DefaultCheckIdentity = true;

        /// <summary>false</summary>
        private const bool DefaultCheckParents = false;

        /// <summary>true</summary>
        private const bool DefaultCheckMirrors = true;

        /// <summary>true</summary>
        private const bool DefaultCheckDownloads = true;

        /// <summary>
        /// Check whether the <see cref="Partition.Identity"/>(-ies) are equal.
        /// The default value is <inheritdoc cref="DefaultCheckIdentity"/>.
        /// </summary>
        public bool CheckIdentity { get; set; } = DefaultCheckIdentity;

        /// <summary>
        /// Check whether the <see cref="Partition.Episode"/>s are equal.
        /// The default value is <inheritdoc cref="DefaultCheckParents"/>.
        /// </summary>
        public bool CheckParents { get; set; } = DefaultCheckParents;

        /// <summary>
        /// Check whether the <see cref="Partition.CheckMirrors"/> are equal.
        /// The default value is <inheritdoc cref="DefaultCheckMirrors"/>.
        /// </summary>
        public bool CheckMirrors { get; set; } = DefaultCheckMirrors;

        /// <summary>
        /// Check whether the <see cref="Partition.Download"/>s are equal.
        /// The default value is <inheritdoc cref="DefaultCheckDownloads"/>.
        /// </summary>
        public bool CheckDownloads { get; set; } = DefaultCheckDownloads;
    }
}
