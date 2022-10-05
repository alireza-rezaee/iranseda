using HtmlAgilityPack;
using Rezaee.Data.Iranseda.Configurations;
using Rezaee.Data.Iranseda.Exceptions;
using Rezaee.Data.Iranseda.Extensions;
using Rezaee.Data.Iranseda.Helpers;
using Rezaee.Data.Iranseda.JsonConverters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Rezaee.Data.Iranseda
{
    /// <summary>
    /// Details of a episode that belongs to an <see cref="Iranseda.Programme"/>.
    /// </summary>
    public class Episode : BaseCatalogue<Episode?>
    {
        #region Fields
        private DateTime _lastModified;
        #endregion

        #region Properties
        /// <summary>
        /// A unique identifier that can be seen in the <see cref="Url">Url</see> of this episode's page in Iranseda.
        /// </summary>
        public string Id
        {
            get
            {
                var queryString = HttpUtility.ParseQueryString(Url.Query);
                if (queryString.AllKeys.Contains("e"))
                    return queryString["e"];
                else
                    throw new Exception($"The {nameof(Url)} does not contain the \"e\" parameter. {nameof(Url)}: \"{Url}\"");
            }
        }

        /// <summary>
        /// The name of the current episode.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The date when this episode is recorded.
        /// </summary>
        [JsonConverter(typeof(DateLocalJsonConverter))]
        public DateTime Date { get; set; }

        /// <summary>
        /// The URL of the current episode.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// Partitions in the current episode.
        /// </summary>
        public List<Partition>? Partitions { get; set; }

        /// <inheritdoc/>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public override DateTime LastModified
        {
            get
            {
                if (Partitions != null && Partitions.Any())
                {
                    var partitionsLastModified = Partitions.OrderByDescending(p => p.LastModified).First().LastModified;
                    if (partitionsLastModified >= _lastModified)
                        return _lastModified = partitionsLastModified;
                }
                return _lastModified;
            }
            set => _lastModified = value;
        }

        /// <summary>
        /// The <see cref="Url">Url</see> property of the current episode is used as its unique identifier.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        internal Uri Identity { get => Url; }

        /// <summary>
        /// The <see cref="Iranseda.Programme">programme</see> that this episode belongs to.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Programme? Programme { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new <see cref="Episode"/> instance with <paramref name="url"/>,
        /// <paramref name="name"/>, <paramref name="date"/>, and the optional desired
        /// <paramref name="partitions"/> in it.
        /// </summary>
        /// <param name="url">The URL of this episode.</param>
        /// <param name="name">The name of this episode.</param>
        /// <param name="date">The broadcast date of this episode.</param>
        /// <param name="partitions">The desired partitions to be included in this episode.</param>
        public Episode(Uri url, string name, DateTime date, List<Partition>? partitions = null)
            => (Url, Name, Date, Partitions) = (url, name, date, partitions);

        /// <summary>
        /// Create a new <see cref="Episode"/> instance with <paramref name="url"/>,
        /// <paramref name="name"/>, <paramref name="date"/>, <paramref name="lastModified"/>
        /// and the optional desired <paramref name="partitions"/> in it.
        /// </summary>
        /// <param name="url">The URL of this episode.</param>
        /// <param name="name">The name of this episode.</param>
        /// <param name="date">The broadcast date of this episode.</param>
        /// <param name="lastModified">The date and time of the last changes applied to this episode.</param>
        /// <param name="partitions">The desired partitions to be included in this episode.</param>
        [JsonConstructor]
        public Episode(Uri url, string name, DateTime date, DateTime lastModified, List<Partition>? partitions = null)
            => (Url, Name, Date, LastModified, Partitions) = (url, name, date, lastModified, partitions);
        #endregion

        #region Methods
        /// <summary>
        /// Load all the partitions of this episode from <see href="http://radio.iranseda.ir/">Iranseda</see> website.
        /// </summary>
        /// <param name="tries">The number of attempts to send the request, if the previous one failed.</param>
        /// <param name="skipHtmlException">Prevent <see cref="HtmlParseException"/> from being thrown.</param>
        /// <param name="proxy">Proxy to use with the request.</param>
        /// <param name="credentials">Credentials to use when authenticating.</param>
        /// <returns>List of all the available partitions for the current episode.</returns>
        public List<Partition>? LoadPartitions(int tries = 1, bool skipHtmlException = false,
            WebProxy? proxy = null, NetworkCredential? credentials = null)
        {
            for (int @try = 0; @try < tries; @try++)
            {
                try
                {
                    List<Partition> parts = new List<Partition>();

                    HtmlWeb htmlWeb = new HtmlWeb()
                    {
                        AutoDetectEncoding = false,
                        OverrideEncoding = System.Text.Encoding.UTF8
                    };

                    HtmlDocument htmlDoc = htmlWeb.Load(Url.AbsoluteUri, "GET", proxy, credentials);

                    HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode
                        .SelectNodes("(//section[@id='taglist2']//div[contains(@class, 'container')]/div/div)[2]//article");

                    if (htmlNodes == null || !htmlNodes.Any())
                    {
                        if (skipHtmlException)
                            return parts;
                        throw new HtmlParseException("Reached incorrect HTML format or the target"
                            + $"element(s) did not exist on the page. (Episode-URL: {Url})");
                    }

                    foreach (HtmlNode node in htmlNodes)
                    {
                        List<Uri> partMirrorUris = new List<Uri>();

                        var timeString = node.SelectSingleNode(".//span[contains(@class, 'start_time')]").InnerText.Trim()
                            .TranslateDigits(from: new CultureInfo("fa-IR"), to: new CultureInfo("en"));

                        var modalId = node.SelectSingleNode(".//a[contains(@data-toggle, 'modal')]").Attributes["href"].Value.Trim()[1..];

                        var baseUri = Url;
                        HtmlNodeCollection mirrorUriNodes = htmlDoc.DocumentNode.SelectNodes($"//div[@id='{modalId}']//a");
                        foreach (HtmlNode uriNode in mirrorUriNodes)
                        {
                            if (Uri.TryCreate(baseUri, uriNode.Attributes["href"].Value.Trim(), out Uri? mirrirUri) && mirrirUri != null)
                                partMirrorUris.Add(mirrirUri);
                        }

                        parts.Add(new Partition(timeString, partMirrorUris, DateTime.Now)
                        {
                            Episode = this
                        });
                    }

                    return parts;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads all partitions in this episode recursively.
        /// </summary>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public async Task DownloadAsync(string directory, Uri? preferredMirror = null, bool skipIfExists = true, HttpClient? client = null)
        {
            if (Partitions == null || !Partitions.Any())
                return;

            await Partition.DownloadPartitionsAsync(
                partitions: Partitions,
                directory: directory,
                skipIfExists: skipIfExists,
                preferredMirror: preferredMirror);
        }

        /// <summary>
        /// Delete all downloaded files of this episode recursively.
        /// </summary>
        public void ClearDownloads()
        {
            if (Partitions != null && Partitions.Any())
                foreach (Partition partition in Partitions)
                    if (partition.IsDownloadExist())
                        partition.ClearDownload();
        }

        /// <summary>
        /// Merges the contents of several <see cref="Episode"/>s together.
        /// </summary>
        /// <param name="episodes">The desired <see cref="Episode"/>s for merge.</param>
        /// <returns>Returns a new <see cref="Episode"/> as the result of the merge process.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Episode Merge(IEnumerable<Episode> episodes)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(episodes, nameof(episodes));

            if (!episodes.Any())
                throw new ArgumentException(message: "No element is specified for merge.", paramName: nameof(episodes));

            if (episodes.Count() == 1)
                return episodes.First();
            else if (episodes.Count() == 2)
                return Merge(episodes.ElementAt(0), episodes.ElementAt(1));

            Episode merged = episodes.First();
            foreach (var episode in episodes.Skip(1))
                merged = Merge(merged, episode);
            return merged;
        }

        /// <summary>
        /// Merges the contents of two <see cref="Episode"/>s together.
        /// </summary>
        /// <param name="first">First item to merge.</param>
        /// <param name="second">Second item to merge.</param>
        /// <returns>Returns a new <see cref="Episode"/> as the result of the merge process.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Episode Merge(Episode first, Episode second)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(first, nameof(first));
            ThrowHelper.ThrowArgumentNullExceptionIfNull(second, nameof(second));

            if (first.Identity != second.Identity)
                throw new InvalidOperationException($"Merge Error. Both side of merge must have a same {nameof(first.Identity)}.");

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return newer;

            List<Partition> mergedPartitions = newer.Partitions.Union(older.Partitions).GroupBy(partition => partition.Identity)
                .Select(partitions => Partition.Merge(partitions)).OrderBy(partition => partition.Time).ToList();

            return new Episode(newer.Url, newer.Name, newer.Date, partitions: mergedPartitions);
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Episode"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <param name="config">Apply equality configuration.</param>
        /// <returns>Are they equal?</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool Equals(Episode? left, Episode? right, EpisodesEqualityConfiguration config)
        {
            switch (NullHelper.NullComparison(left, right))
            {
                case NullComparisonResult.BothNull:
                    return true;
                case NullComparisonResult.OneSideOnly:
                    return false;
                case NullComparisonResult.NoneNull:
                    if (config.CheckIdentity && left!.Identity != right!.Identity)
                        return false;

                    if (config.CheckPartitions)
                        switch (NullHelper.NullComparison(left!.Partitions, right!.Partitions))
                        {
                            case NullComparisonResult.BothNull:
                                break;
                            case NullComparisonResult.OneSideOnly:
                                return false;
                            case NullComparisonResult.NoneNull:
                                if (left.Partitions.Distinct().Count() != right.Partitions.Distinct().Count())
                                    return false;

                                if (left.Partitions.Any(fp => !right.Partitions.Any(sp => Partition.Equals(fp, sp, config.PartitionsConfig)))
                                    || right.Partitions.Any(sp => !left.Partitions.Any(fp => Partition.Equals(fp, sp, config.PartitionsConfig))))
                                    return false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    if (config.CheckParents && left!.Programme != right!.Programme)
                        return false;

                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Episode"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they equal?</returns>
        public static bool operator ==(Episode? left, Episode? right)
            => Equals(left, right, new EpisodesEqualityConfiguration());

        /// <summary>
        /// It is used to compare the inequality of two <see cref="Episode"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they not equal?</returns>
        public static bool operator !=(Episode? left, Episode? right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (Episode)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Identity, Partitions?.GetOrderIndependentHashCode()).GetHashCode();
        #endregion
    }
}