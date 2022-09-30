namespace Rezaee.Data.Iranseda.Configurations
{
    /// <summary>
    /// Get or set equality configuration for <see cref="Channel"/>s comparison
    /// </summary>
    public class ChannelsEqualityConfiguration : IEqualityConfiguration
    {
        /// <summary>true</summary>
        private const bool DefaultCheckIdentity = true;

        /// <summary>false</summary>
        private const bool DefaultCheckParents = false;

        /// <summary>true</summary>
        private const bool DefaultCheckProgrammes = true;

        /// <summary>
        /// Check whether the <see cref="Channel.Identity"/>(-ies) are equal.
        /// The default value is <inheritdoc cref="DefaultCheckIdentity"/>.
        /// </summary>
        public bool CheckIdentity { get; set; } = DefaultCheckIdentity;

        /// <summary>
        /// Check whether the <see cref="Channel.Catalogue"/>s are equal.
        /// The default value is <inheritdoc cref="DefaultCheckParents"/>.
        /// </summary>
        public bool CheckParents { get; set; } = DefaultCheckParents;

        /// <summary>
        /// Check whether the <see cref="Channel.Programmes"/> are equal.
        /// The default value is <inheritdoc cref="DefaultCheckProgrammes"/>.
        /// </summary>
        public bool CheckProgrammes { get; set; } = DefaultCheckProgrammes;

        /// <inheritdoc cref="ProgrammesEqualityConfiguration"/>
        public ProgrammesEqualityConfiguration ProgrammesConfig { get; set; } = new ProgrammesEqualityConfiguration();
    }
}
