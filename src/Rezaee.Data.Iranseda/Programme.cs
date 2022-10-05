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
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Rezaee.Data.Iranseda
{
    /// <summary>
    /// Details of a programme that belongs to an <see cref="Iranseda.Channel"/>.
    /// </summary>
    public class Programme : BaseCatalogue<Programme?>
    {
        #region Fields
        private Channel? _channel;
        private DateTime _lastModified;
        #endregion

        #region Properties
        /// <summary>
        /// TODO
        /// </summary>
        public Identity Identity { get; set; }

        /// <summary>
        /// The name of the current programme.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The URL of the current programme.
        /// </summary>
        public Uri Url { get => UrlHelper.MakeProgrammeUrl(ch: Identity.ChannelId, m: Identity.Id); }

        /// <summary>
        /// Episodes in the current programme.
        /// </summary>
        public List<Episode>? Episodes { get; set; }

        /// <inheritdoc/>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public override DateTime LastModified
        {
            get
            {
                if (Episodes != null && Episodes.Any())
                {
                    var partitionsLastModified = Episodes.OrderByDescending(e => e.LastModified).First().LastModified;
                    if (partitionsLastModified >= _lastModified)
                        return _lastModified = partitionsLastModified;
                }
                return _lastModified;
            }
            set => _lastModified = value;
        }

        /// <summary>
        /// The <see cref="Iranseda.Channel">channel</see> that this programme belongs to.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Channel Channel
        {
            get => _channel ?? new Channel(id: Identity.ChannelId);
            set => _channel = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="episodes"></param>
        public Programme(string channelId,
            string id,
            string? name = null,
            List<Episode>? episodes = null)
            => (Identity, Name, Episodes) = (new Identity(channelId, id), name, episodes);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="name"></param>
        /// <param name="episodes"></param>
        public Programme(Identity identity,
            string? name = null,
            List<Episode>? episodes = null)
            => (Identity, Name, Episodes) = (identity, name, episodes);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="channel"></param>
        /// <param name="name"></param>
        /// <param name="episodes"></param>
        public Programme(string id,
            Channel channel,
            string? name = null,
            List<Episode>? episodes = null)
            => (Identity, Channel, Name, Episodes) = (new Identity(channel.Id, id), channel, name, episodes);
        #endregion

        #region Methods
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public List<Episode>? LoadEpisodes(LoadOptions? options = null, bool isRecursive = false)
        {
            options ??= new LoadOptions();

            for (int @try = 0; @try < options.MaxTries; @try++)
            {
                try
                {
                    List<Episode> episodes = new List<Episode>();
                    var persianCulture = new CultureInfo("fa-IR");

                    HtmlWeb htmlWeb = new HtmlWeb()
                    {
                        AutoDetectEncoding = false,
                        OverrideEncoding = System.Text.Encoding.UTF8
                    };

                    HtmlDocument htmlDoc = htmlWeb.Load(Url.AbsoluteUri, "GET", options.Proxy, options.Credential);

                    HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'episode-box')]//li");

                    if (htmlNodes == null || !htmlNodes.Any())
                    {
                        if (options.DontThrowHPE)
                            return episodes;
                        throw new HtmlParseException("Reached incorrect HTML format or the target"
                            + $"element(s) did not exist on the page. (Programme-URL: {Url})");
                    }

                    foreach (var node in htmlNodes)
                    {
                        string episodeId = string.Empty;
                        string episodeName = node.SelectSingleNode(".//p[contains(@class, 'ott-name')]").InnerText.Trim();

                        string dateString = Regex.Replace(node.SelectSingleNode(".//div[contains(@class, 'dt-title')]")
                            .InnerText.Trim(), @"\s+", " ");
                        DateTime episodeDate = DateTime.Parse(dateString, persianCulture);

                        Uri baseUri = Url;
                        string inputUrl = node.SelectSingleNode(".//a[contains(@class, 'page-loding')]").Attributes["href"].Value.Trim();
                        if (Uri.TryCreate(baseUri, inputUrl, out Uri? episodeUri) && episodeUri != null)
                        {
                            var queryString = HttpUtility.ParseQueryString(episodeUri.Query);
                            if (queryString.AllKeys.Contains("e"))
                                episodeId = queryString["e"] ?? string.Empty;
                        }
                        else
                            throw new InvalidOperationException("Could not find one or more episode URL.");

                        episodes.Add(new Episode(id: episodeId, programme: this, name: episodeName, date: episodeDate)
                        {
                            LastModified = DateTime.Now
                        });
                    }

                    if (isRecursive)
                        episodes.ForEach(e => e.Partitions = e.LoadPartitions(options));

                    return episodes;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads all partitions in this programme recursively.
        /// </summary>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public async Task DownloadAsync(string directory, Uri? preferredMirror = null, bool skipIfExists = true, HttpClient? client = null)
        {
            var partitions = Episodes.SelectMany(Episode => Episode.Partitions);

            if (partitions == null || !partitions.Any())
                return;

            await Partition.DownloadPartitionsAsync(
                partitions: partitions,
                directory: directory,
                skipIfExists: skipIfExists,
                preferredMirror: preferredMirror);
        }

        /// <summary>
        /// Delete all downloaded files of this programme recursively.
        /// </summary>
        public void ClearDownloads()
        {
            if (Episodes != null && Episodes.Any())
                foreach (Episode episode in Episodes)
                    episode.ClearDownloads();
        }

        /// <summary>
        /// Merges the contents of several <see cref="Programme"/>s together.
        /// </summary>
        /// <param name="programmes">The desired <see cref="Programme"/>s for merge.</param>
        /// <returns>Returns a new <see cref="Programme"/> as the result of the merge process.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Programme Merge(IEnumerable<Programme> programmes)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(programmes, nameof(programmes));

            if (!programmes.Any())
                throw new ArgumentException(message: "No element is specified for merge.", paramName: nameof(programmes));

            if (programmes.Count() == 1)
                return programmes.First();
            else if (programmes.Count() == 2)
                return Merge(programmes.ElementAt(0), programmes.ElementAt(1));

            Programme merged = programmes.First();
            foreach (var programme in programmes.Skip(1))
                merged = Merge(merged, programme);
            return merged;
        }

        /// <summary>
        /// Merges the contents of two <see cref="Programme"/>s together.
        /// </summary>
        /// <param name="first">First item to merge.</param>
        /// <param name="second">Second item to merge.</param>
        /// <returns>Returns a new <see cref="Programme"/> as the result of the merge process.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Programme Merge(Programme first, Programme second)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(first, nameof(first));
            ThrowHelper.ThrowArgumentNullExceptionIfNull(second, nameof(second));

            if (first.Identity != second.Identity)
                throw new InvalidOperationException($"Merge Error. Both side of merge must have a same {nameof(first.Identity)}.");

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return newer;

            List<Episode> mergedEpisodes = newer.Episodes.Union(older.Episodes).GroupBy(episode => episode.Identity)
                .Select(episodes => Episode.Merge(episodes)).OrderByDescending(episode => episode.Date).ToList();

            return new Programme(identity: newer.Identity, name: newer.Name, episodes: mergedEpisodes)
            {
                LastModified = newer.LastModified
            };
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Programme"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <param name="config">Apply equality configuration.</param>
        /// <returns>Are they equal?</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool Equals(Programme? left, Programme? right, ProgrammesEqualityConfiguration config)
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

                    if (config.CheckEpisodes)
                    {
                        switch (NullHelper.NullComparison(left!.Episodes, right!.Episodes))
                        {
                            case NullComparisonResult.BothNull:
                                break;
                            case NullComparisonResult.OneSideOnly:
                                return false;
                            case NullComparisonResult.NoneNull:
                                if (left.Episodes.Distinct().Count() != right.Episodes.Distinct().Count())
                                    return false;

                                if (left.Episodes.Any(le => !right.Episodes.Any(re => Episode.Equals(le, re, config.EpisodesConfig)))
                                || right.Episodes.Any(re => !left.Episodes.Any(le => Episode.Equals(le, re, config.EpisodesConfig))))
                                    return false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }

                    if (config.CheckParents && left!.Channel != right!.Channel)
                        return false;

                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Programme"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they equal?</returns>
        public static bool operator ==(Programme? left, Programme? right)
            => Equals(left, right, new ProgrammesEqualityConfiguration());

        /// <summary>
        /// It is used to compare the inequality of two <see cref="Programme"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they not equal?</returns>
        public static bool operator !=(Programme? left, Programme? right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (Programme)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Identity, Episodes?.GetOrderIndependentHashCode()).GetHashCode();
        #endregion
    }
}
