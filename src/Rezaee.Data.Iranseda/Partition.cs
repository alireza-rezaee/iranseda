using Rezaee.Data.Iranseda.Configurations;
using Rezaee.Data.Iranseda.Extensions;
using Rezaee.Data.Iranseda.Helpers;
using Rezaee.Data.Iranseda.JsonConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json.Serialization;
using System.Threading;
using System.Threading.Tasks;

namespace Rezaee.Data.Iranseda
{
    /// <summary>
    /// Details of a partition that belongs to an <see cref="Iranseda.Episode"/>.
    /// </summary>
    public class Partition : BaseCatalogue<Partition?>
    {
        #region Fields
        private DateTime _lastModified;
        #endregion

        #region Properties
        /// <summary>
        /// The time when this partition is recorded.
        /// </summary>
        public string Time { get; set; }

        /// <summary>
        /// Mirrors to download the file of this partition.
        /// </summary>
        public List<Uri> Mirrors { get; set; }

        /// <summary>
        /// Download information for the current partition.
        /// </summary>
        public Download? Download { get; set; }

        /// <inheritdoc/>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public override DateTime LastModified
        {
            get
            {
                if (Download != null && Download.LastModified >= _lastModified)
                    return _lastModified = Download.LastModified;
                return _lastModified;
            }
            set => _lastModified = value;
        }

        /// <summary>
        /// The <see cref="Time">Time</see> property of the current partition is used as its unique identifier.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        internal string Identity { get => Time; }

        /// <summary>
        /// The <see cref="Iranseda.Episode">episode</see> that this partition belongs to.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Episode? Episode { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new <see cref="Partition"/> instance with <paramref name="time"/>,
        /// <paramref name="mirrors"/> and the optional <paramref name="download"/>.
        /// </summary>
        /// <param name="time">The broadcast time of this partition.</param>
        /// <param name="mirrors">The mirrors to download the file of this partition.</param>
        /// <param name="download">Information about the downloaded file of this partition.</param>
        public Partition(string time, List<Uri> mirrors, Download? download = null)
            => (Mirrors, Time, Download) = (mirrors, time, download);

        /// <summary>
        /// Create a new <see cref="Partition"/> instance with <paramref name="time"/>,
        /// <paramref name="mirrors"/>, <paramref name="lastModified"/> and the optional
        /// <paramref name="download"/>.
        /// </summary>
        /// <param name="time">The broadcast time of this partition.</param>
        /// <param name="mirrors">The mirrors to download the file of this partition.</param>
        /// <param name="lastModified">The date and time of the last changes applied to this partition.</param>
        /// <param name="download">Information about the downloaded file of this partition.</param>
        [JsonConstructor]
        public Partition(string time, List<Uri> mirrors, DateTime lastModified, Download? download = null)
            => (Mirrors, Time, LastModified, Download) = (mirrors, time, lastModified, download);
        #endregion

        #region Methods
        /// <summary>
        /// Download this partition.
        /// </summary>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="filename">The desired file name to overwrite the default name.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public async Task DownloadAsync(string directory,
            string? relativeToPath = null,
            bool skipIfExists = true,
            Uri? preferredMirror = null,
            string? filename = null,
            HttpClient? client = null)
        {
            await DownloadPartitionAsync(
                partition: this,
                directory: directory,
                relativeToPath: relativeToPath ?? directory,
                skipIfExists: skipIfExists,
                preferredMirror: preferredMirror,
                filename: filename,
                client: client);
        }

        /// <summary>
        /// Download multiple <see cref="Partition"/>s in parallel.
        /// </summary>
        /// <param name="partitions">The desired partitions for download.</param>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="limit">Maximum number of simultaneous downloads.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public static async Task DownloadPartitionsAsync(IEnumerable<Partition> partitions,
            string directory,
            string? relativeToPath = null,
            bool skipIfExists = true,
            Uri? preferredMirror = null,
            int limit = 10,
            HttpClient? client = null)
        {
            client ??= new HttpClient();
            using var semaphore = new SemaphoreSlim(limit, limit);
            var tasks = partitions.Select(async partition =>
            {
                await semaphore.WaitAsync().ConfigureAwait(false);
                try
                {
                    relativeToPath ??= directory;
                    await DownloadPartitionAsync(partition, directory, relativeToPath, skipIfExists, preferredMirror, client: client);
                }
                finally
                {
                    semaphore.Release();
                }
            }).ToArray();
            await Task.WhenAll(tasks).ConfigureAwait(false);
        }

        /// <summary>
        /// Download this <see cref="Partition"/> to the target directory and optional filename.
        /// </summary>
        /// <param name="partition">The desired partition for download.</param>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="filename">The desired file name to overwrite the default name.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        /// <returns>Downloaded file information as <see cref="Iranseda.Download"/>.</returns>
        /// <exception cref="ArgumentNullException"></exception>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        /// <exception cref="Exception"></exception>
        private static async Task<Download> DownloadPartitionAsync(Partition partition,
            string directory,
            string? relativeToPath = null,
            bool skipIfExists = true,
            Uri? preferredMirror = null,
            string? filename = null,
            HttpClient? client = null)
        {
            if (string.IsNullOrEmpty(directory))
                throw new ArgumentNullException(nameof(directory));
            if (!string.IsNullOrEmpty(filename?.Trim()) && filename.Any(c => Path.GetInvalidFileNameChars().Contains(c)))
                throw new InvalidOperationException($"The specified {filename} contains invalid character(s).");
            if (!Directory.Exists(directory))
                throw new ArgumentException($"There is not any directory at the specified path. (Path: \"{directory}\")");

            if (partition.IsDownloadExist())
            {
                if (skipIfExists)
                    return partition.Download!;
                else
                    partition.ClearDownload();
            }

            if (client == null)
            {
                client = new HttpClient();
                client.DefaultRequestHeaders.Accept.Clear();
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("video/mp4"));
                client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("audio/mpeg"));
            }

            IEnumerable<Uri> mirrors;
            if (preferredMirror == null)
                mirrors = partition.Mirrors;
            else
                mirrors = partition.Mirrors.OrderByDescending(mirror => preferredMirror.IsBaseOf(mirror));

            foreach (Uri mirror in mirrors)
            {
                try
                {
                    using var response = await client.GetAsync(mirror).ConfigureAwait(false);
                    response.EnsureSuccessStatusCode();

                    if (string.IsNullOrEmpty(filename))
                    {
                        string? contentDispositionFileName = response.Content.Headers.GetValues("Content-Disposition")
                        .FirstOrDefault(c => c.Contains("filename="))?.Split("filename=")?.FirstOrDefault(i => !i.Contains("attachment"));
                        if (!string.IsNullOrEmpty(contentDispositionFileName))
                            filename = contentDispositionFileName;
                        else
                            filename = new Guid().ToString();
                        filename = string.Join("_", filename.Split(Path.GetInvalidFileNameChars()));
                    }

                    string filePath = Path.Combine(directory, filename);
                    using var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);
                    using var fs = new FileStream(filePath, FileMode.CreateNew);
                    await response.Content.CopyToAsync(fs);

                    relativeToPath ??= directory;
                    string fileRelativePath = Path.GetRelativePath(relativeToPath, filePath);
                    return partition.Download = new Download(url: mirror, path: fileRelativePath, lastModified: DateTime.Now);
                }
                catch (Exception)
                {

                }
            }

            throw new Exception("All mirrors were tried but unfortunately the download encountered an unexpected error.");
        }

        /// <summary>
        /// Check if a file has been downloaded for this partition.
        /// </summary>
        /// <returns>Was there a file?</returns>
        public bool IsDownloadExist()
        {
            if (Download == null || string.IsNullOrEmpty(Download.Path))
                return false;
            return File.Exists(Download.Path);
        }

        /// <summary>
        /// Delete the downloaded file of this partition recursively.
        /// </summary>
        public void ClearDownload()
            => File.Delete(Download?.Path);

        /// <summary>
        /// Merges the contents of several <see cref="Partition"/>s together.
        /// </summary>
        /// <param name="partitions">The desired <see cref="Partition"/>s for merge.</param>
        /// <returns>Returns a new <see cref="Partition"/> as the result of the merge process.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Partition Merge(IEnumerable<Partition> partitions)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(partitions, nameof(partitions));

            if (!partitions.Any())
                throw new ArgumentException(message: "No element is specified for merge.", paramName: nameof(partitions));

            if (partitions.Count() == 1)
                return partitions.First();
            else if (partitions.Count() == 2)
                return Merge(partitions.ElementAt(0), partitions.ElementAt(1));

            Partition merged = partitions.First();
            foreach (var partition in partitions.Skip(1))
                merged = Merge(merged, partition);
            return merged;
        }

        /// <summary>
        /// Merges the contents of two <see cref="Partition"/>s together.
        /// </summary>
        /// <param name="first">First item to merge.</param>
        /// <param name="second">Second item to merge.</param>
        /// <returns>Returns a new <see cref="Partition"/> as the result of the merge process.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Partition Merge(Partition first, Partition second)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(first, nameof(first));
            ThrowHelper.ThrowArgumentNullExceptionIfNull(second, nameof(second));

            if (first.Identity != second.Identity)
                throw new InvalidOperationException($"Merge Error. Both side of merge must have a same {nameof(first.Identity)}.");

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return newer;

            List<Uri> mergedMirrors = newer.Mirrors.Union(older.Mirrors).Distinct().ToList();

            Download? mergedDownload = null;
            switch (NullHelper.NullComparison(first.Download, second.Download))
            {
                case NullComparisonResult.BothNull:
                    mergedDownload = null;
                    break;
                case NullComparisonResult.OneSideOnly:
                    mergedDownload = first.Download ?? second.Download;
                    break;
                case NullComparisonResult.NoneNull:
                    string? path = null;
                    Uri? url = null;

                    if (!string.IsNullOrEmpty(newer.Download?.Path) && File.Exists(newer.Download?.Path))
                        (path, url) = (newer.Download!.Path, newer.Download!.Url);
                    else if (!string.IsNullOrEmpty(older.Download?.Path) && File.Exists(older.Download?.Path))
                        (path, url) = (older.Download!.Path, older.Download!.Url);

                    if (url != null && path != null)
                        mergedDownload = new Download(url, path);
                    break;
            }

            return new Partition(newer.Time, mirrors: mergedMirrors, download: mergedDownload, lastModified: newer.LastModified);
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Partition"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <param name="config">Apply equality configuration.</param>
        /// <returns>Are they equal?</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool Equals(Partition? left, Partition? right, PartitionsEqualityConfiguration config)
        {
            var nullStatus = NullHelper.NullComparison(left, right);

            switch (NullHelper.NullComparison(left, right))
            {
                case NullComparisonResult.BothNull:
                    return true;
                case NullComparisonResult.OneSideOnly:
                    return false;
                case NullComparisonResult.NoneNull:
                    if (config.CheckIdentity && left!.Identity != right!.Identity)
                        return false;

                    if (config.CheckMirrors)
                    {
                        if (left!.Mirrors.Count != right!.Mirrors.Count)
                            return false;

                        if (left.Mirrors.Any(f => !right.Mirrors.Contains(f))
                            || right.Mirrors.Any(s => !left.Mirrors.Contains(s)))
                            return false;
                    }

                    if (config.CheckDownloads && left!.Download! != right!.Download!)
                        return false;

                    if (config.CheckParents && left!.Episode != right!.Episode)
                        return false;

                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Partition"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they equal?</returns>
        public static bool operator ==(Partition? left, Partition? right)
            => Equals(left, right, new PartitionsEqualityConfiguration());

        /// <summary>
        /// It is used to compare the inequality of two <see cref="Partition"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they not equal?</returns>
        public static bool operator !=(Partition? left, Partition? right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (Partition)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Identity, Download, Mirrors.GetOrderIndependentHashCode()).GetHashCode();
        #endregion
    }
}