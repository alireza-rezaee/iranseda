using HtmlAgilityPack;
using Rezaee.Iranseda.Configurations;
using Rezaee.Iranseda.Exceptions;
using Rezaee.Iranseda.Extensions;
using Rezaee.Iranseda.Helpers;
using Rezaee.Iranseda.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Rezaee.Iranseda
{
    /// <summary>
    /// Details of a channel that belongs to an <see cref="Iranseda.Catalogue"/>.
    /// </summary>
    public class Channel : BaseCatalogue<Channel?>
    {
        private DateTime _lastModified;

        /// <summary>
        /// The URL of the current channel.
        /// </summary>
        public Uri Url { get; set; }

        /// <summary>
        /// A unique identifier that can be seen in the <see cref="Url">Url</see> of this channel's page in Iranseda.
        /// </summary>
        public string Id {
            get
            {
                var queryString = HttpUtility.ParseQueryString(Url.Query);
                if (queryString.AllKeys.Contains("ch"))
                    return queryString["ch"];
                else
                    throw new Exception($"The {nameof(Url)} does not contain the \"ch\" parameter. {nameof(Url)}: \"{Url}\"");
            }
        }

        /// <summary>
        /// The name of the current channel.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The URL of the list page of all programmes within the current channel.
        /// </summary>
        public Uri ProgrammesUrl
        {
            get => new Uri(Url.ToString().Replace("live", "programlist"));
        }

        /// <inheritdoc/>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public override DateTime LastModified
        {
            get
            {
                if (Programmes != null && Programmes.Any())
                {
                    var partitionsLastModified = Programmes.OrderByDescending(p => p.LastModified).First().LastModified;
                    if (partitionsLastModified >= _lastModified)
                        return _lastModified = partitionsLastModified;
                }
                return _lastModified;
            }
            set => _lastModified = value;
        }

        /// <summary>
        /// The <see cref="Id">Id</see> property of the current channel is used as its unique identifier.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public string Identity { get => Id; }

        /// <summary>
        /// The <see cref="Iranseda.Catalogue">catalogue</see> that this channel belongs to.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Catalogue? Catalogue { get; set; }

        /// <summary>
        /// Programmes in the current channel.
        /// </summary>
        public List<Programme>? Programmes { get; set; }

        /// <summary>
        /// Create a new <see cref="Channel"/> instance with <paramref name="url"/>,
        /// <paramref name="name"/> and the optional desired <paramref name="programmes"/> in it.
        /// </summary>
        /// <param name="url">The URL of this channel.</param>
        /// <param name="name">The name of this channel.</param>
        /// <param name="programmes">The desired programmes to be included in this channel.</param>
        public Channel(Uri url, string name, List<Programme>? programmes = null)
            => (Name, Url, LastModified, Programmes) = (name, url, DateTime.Now, programmes);

        /// <summary>
        /// Create a new <see cref="Channel"/> instance with <paramref name="url"/>,
        /// <paramref name="name"/>, <paramref name="lastModified"/> and the optional desired
        /// <paramref name="programmes"/> in it.
        /// </summary>
        /// <param name="url">The URL of this channel.</param>
        /// <param name="name">The name of this channel.</param>
        /// <param name="lastModified">The date and time of the last changes applied to this channel.</param>
        /// <param name="programmes">The desired programmes to be included in this channel.</param>
        [JsonConstructor]
        public Channel(Uri url, string name, DateTime lastModified, List<Programme>? programmes = null)
            => (Name, Url, LastModified, Programmes) = (name, url, lastModified, programmes);

        /// <summary>
        /// Load all the programmes of this channel from <see href="http://radio.iranseda.ir/">Iranseda</see> website.
        /// </summary>
        /// <param name="recursive">Load all the <see cref="Programme"/>s and their childs recursively.</param>
        /// <param name="tries">The number of attempts to send the request, if the previous one failed.</param>
        /// <param name="skipHtmlException">Prevent <see cref="HtmlParseException"/> from being thrown.</param>
        /// <param name="proxy">Proxy to use with the request.</param>
        /// <param name="credentials">Credentials to use when authenticating.</param>
        /// <returns>List of all the available programmes for the current channel.</returns>
        public List<Programme>? LoadProgrammes(bool recursive = false, int tries = 1, bool skipHtmlException = false,
            WebProxy? proxy = null, NetworkCredential? credentials = null)
        {
            for (int @try = 0; @try < tries; @try++)
            {
                try
                {
                    List<Programme> programmes = new List<Programme>();

                    HtmlWeb htmlWeb = new HtmlWeb()
                    {
                        AutoDetectEncoding = false,
                        OverrideEncoding = System.Text.Encoding.UTF8
                    };

                    HtmlDocument htmlDoc = htmlWeb.Load(ProgrammesUrl.AbsoluteUri, "GET", proxy, credentials);

                    HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//section[@id='ProgramList']//article");

                    if (htmlNodes == null || !htmlNodes.Any())
                    {
                        if (skipHtmlException)
                            return programmes;
                        throw new HtmlParseException("Reached incorrect HTML format or the target"
                            + $"element(s) did not exist on the page. (Channel-URL: {Url})");
                    }

                    foreach (var node in htmlNodes)
                    {
                        string programmeId = string.Empty;
                        string programmeName = node.SelectSingleNode(".//h4[contains(@class, 'ott-name')]").InnerText.Trim();

                        Uri baseUri = Url;
                        string inputUrl = node.SelectSingleNode(".//a[contains(@class, 'page-loding')]").Attributes["href"].Value.Trim();
                        if (Uri.TryCreate(baseUri, inputUrl, out Uri? programmeUri) && programmeUri != null)
                        {
                            var queryString = HttpUtility.ParseQueryString(programmeUri.Query);
                            if (queryString.AllKeys.Contains("m"))
                                programmeId = queryString["m"] ?? string.Empty;
                        }
                        else
                            throw new InvalidOperationException("Could not find one or more programme URL.");

                        programmes.Add(new Programme(programmeUri, programmeName, DateTime.Now)
                        {
                            Channel = this,
                        });
                    }

                    if (recursive)
                        programmes.ForEach(p => p.Episodes = p.LoadEpisodes(recursive, tries, skipHtmlException));

                    return programmes;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads all partitions in this channel recursively.
        /// </summary>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public async Task DownloadAsync(string directory,
            Uri? preferredMirror = null,
            bool skipIfExists = true,
            HttpClient? client = null)
        {
            var partitions = Programmes.SelectMany(programme =>
                programme.Episodes.SelectMany(episode =>
                episode.Partitions));

            if (partitions == null || !partitions.Any())
                return;

            await Partition.DownloadPartitionsAsync(
                partitions: partitions,
                directory: directory,
                skipIfExists: skipIfExists,
                preferredMirror: preferredMirror,
                client: client);
        }

        /// <summary>
        /// Delete all downloaded files of this channel recursively.
        /// </summary>
        public void ClearDownloads()
        {
            if (Programmes != null && Programmes.Any())
                foreach (Programme programme in Programmes)
                    programme.ClearDownloads();
        }

        /// <summary>
        /// Merges the contents of several <see cref="Channel"/>s together.
        /// </summary>
        /// <param name="channels">The desired <see cref="Channel"/>s for merge.</param>
        /// <returns>Returns a new <see cref="Channel"/> as the result of the merge process.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Channel Merge(IEnumerable<Channel> channels)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(channels, nameof(channels));

            if (!channels.Any())
                throw new ArgumentException(message: "No element is specified for merge.", paramName: nameof(channels));

            if (channels.Count() == 1)
                return channels.First();
            else if (channels.Count() == 2)
                return Merge(channels.ElementAt(0), channels.ElementAt(1));

            Channel merged = channels.First();
            foreach (var channel in channels.Skip(1))
                merged = Merge(merged, channel);
            return merged;
        }

        /// <summary>
        /// Merges the contents of two <see cref="Channel"/>s together.
        /// </summary>
        /// <param name="first">First item to merge.</param>
        /// <param name="second">Second item to merge.</param>
        /// <returns>Returns a new <see cref="Channel"/> as the result of the merge process.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public static Channel Merge(Channel first, Channel second)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(first, nameof(first));
            ThrowHelper.ThrowArgumentNullExceptionIfNull(second, nameof(second));

            if (first.Identity != second.Identity)
                throw new InvalidOperationException($"Merge Error. Both side of merge must have a same {nameof(first.Identity)}.");

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return newer;

            List<Programme> mergedProgrammes = newer.Programmes.Union(older.Programmes).GroupBy(programme => programme.Identity)
                .Select(programmes => Programme.Merge(programmes)).OrderBy(programme => programme.Name).ToList();

            return new Channel(url: newer.Url, name: newer.Name, lastModified: newer.LastModified, programmes: mergedProgrammes);
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Channel"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <param name="config">Apply equality configuration.</param>
        /// <returns>Are they equal?</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool Equals(Channel? left, Channel? right, ChannelsEqualityConfiguration config)
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

                    if (config.CheckProgrammes)
                        switch (NullHelper.NullComparison(left!.Programmes, right!.Programmes))
                        {
                            case NullComparisonResult.BothNull:
                                break;
                            case NullComparisonResult.OneSideOnly:
                                return false;
                            case NullComparisonResult.NoneNull:
                                if (left.Programmes.Distinct().Count() != right.Programmes.Distinct().Count())
                                    return false;

                                if (left.Programmes.Any(lp => !right.Programmes.Any(rp => Programme.Equals(lp, rp, config.ProgrammesConfig)))
                                || right.Programmes.Any(rp => !left.Programmes.Any(lp => Programme.Equals(lp, rp, config.ProgrammesConfig))))
                                    return false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }

                    if (config.CheckParents && left!.Catalogue != right!.Catalogue)
                        return false;

                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Channel"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they equal?</returns>
        public static bool operator ==(Channel? left, Channel? right)
            => Equals(left, right, new ChannelsEqualityConfiguration());

        /// <summary>
        /// It is used to compare the inequality of two <see cref="Channel"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they not equal?</returns>
        public static bool operator !=(Channel? left, Channel? right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (Channel)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => (Identity, Programmes?.GetOrderIndependentHashCode()).GetHashCode();
    }
}
