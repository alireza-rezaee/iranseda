using Rezaee.Data.Iranseda.Helpers;
using Rezaee.Data.Iranseda.JsonConverters;
using System;
using System.Text.Json.Serialization;

namespace Rezaee.Data.Iranseda
{
    /// <summary>
    /// Information about a downloaded file.
    /// </summary>
    public class Download
    {
        #region Properties
        /// <summary>
        /// The URL of the downloaded file.
        /// </summary>
        public Uri Url { get; private set; }

        /// <summary>
        /// The path where the downloaded file is saved.
        /// </summary>
        public string Path { get; private set; }

        /// <summary>
        /// The <see cref="Url"/> and <see cref="Path"/> pair is used as a unique identifier.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        internal (Uri Url, string Path) Identity { get => (Url, Path); }

        /// <summary>
        /// When the file is downloaded.
        /// </summary>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public DateTime LastModified { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create an instance of <see cref="Download"/> with <see cref="Url">Url</see> and
        /// <see cref="Path">Path</see> properties of the downloaded file.
        /// </summary>
        /// <param name="url">The URL of the downloaded file.</param>
        /// <param name="path">The path of the downloaded file.</param>
        public Download(Uri url, string path)
            => (Url, Path) = (url, path);

        /// <summary>
        /// Create an instance of <see cref="Download"/> with <see cref="Url">Url</see>, <see cref="Path">Path</see>
        /// and <see cref="LastModified">LastModified</see> properties of the downloaded file.
        /// </summary>
        /// <param name="url">The URL of the downloaded file.</param>
        /// <param name="path">The path of the downloaded file.</param>
        /// <param name="lastModified">When the file is downloaded.</param>
        [JsonConstructor]
        public Download(Uri url, string path, DateTime lastModified)
            => (Url, Path, LastModified) = (url, path, lastModified);
        #endregion

        #region Methods
        /// <summary>
        /// It is used to compare the equality of two <see cref="Download"/>ed files.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they equal?</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool operator ==(Download? left, Download? right)
        {
            var nullStatus = NullHelper.NullComparison(left, right);

            if (nullStatus == NullComparisonResult.BothNull)
                return true;
            else if (nullStatus == NullComparisonResult.OneSideOnly)
                return false;
            else if (nullStatus == NullComparisonResult.NoneNull)
                return left!.Url == right!.Url && left.Path == right.Path;
            else
                throw new ArgumentOutOfRangeException();
        }

        /// <summary>
        /// It is used to compare the inequality of two <see cref="Download"/>ed files.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they not equal?</returns>
        public static bool operator !=(Download? left, Download? right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            Download second = (Download)obj;

            return this == second;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => Identity.GetHashCode();
        #endregion
    }
}