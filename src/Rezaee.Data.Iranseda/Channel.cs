using HtmlAgilityPack;
using Rezaee.Data.Iranseda.Configurations;
using Rezaee.Data.Iranseda.Exceptions;
using Rezaee.Data.Iranseda.Extensions;
using Rezaee.Data.Iranseda.Helpers;
using Rezaee.Data.Iranseda.JsonConverters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Rezaee.Data.Iranseda
{
    /// <summary>
    /// Details of a channel that belongs to an <see cref="Iranseda.Catalogue"/>.
    /// </summary>
    public class Channel : BaseCatalogue<Channel?>
    {
        #region Fields
        private DateTime _lastModified;
        #endregion

        #region Properties
        /// <summary>
        /// TODO
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The name of the current channel.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The URL of the current channel.
        /// </summary>
        public Uri Url { get => UrlHelper.MakeChannelUrl(Id); }

        /// <summary>
        /// Programmes in the current channel.
        /// </summary>
        public List<Programme>? Programmes { get; set; }

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
        /// The <see cref="Iranseda.Catalogue">catalogue</see> that this channel belongs to.
        /// </summary>
        [JsonIgnore(Condition = JsonIgnoreCondition.Always)]
        public Catalogue? Catalogue { get; set; }
        #endregion

        #region Constructors
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        /// <param name="programmes"></param>
        public Channel(string id, string? name = null, List<Programme>? programmes = null)
            => (Id, Name, Programmes) = (id, name, programmes);
        #endregion

        #region Methods
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="options"></param>
        /// <exception cref="Exception"></exception>
        public void LoadProperties(LoadOptions? options = null)
        {
            var loaded = new Catalogue().LoadChannel(c => c.Id == Id);
            if (loaded is null)
                throw new Exception($"An error occurred while loading {nameof(Channel)} properties." +
                    $" ({nameof(Channel)} Id: {Id})");

            // Load properties (except Programmes)
            Name = loaded.Name;
        }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="options"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        /// <exception cref="NotImplementedException"></exception>
        public Programme? LoadProgramme(Func<Programme, bool> predicate, LoadOptions? options = null, bool isRecursive = false)
            => LoadProgrammes(predicate, options, isRecursive).FirstOrDefault();

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="options"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public IEnumerable<Programme>? LoadProgrammes(Func<Programme, bool> predicate, LoadOptions? options = null, bool isRecursive = false)
            => LoadProgrammes(options, isRecursive).Where(predicate);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="options"></param>
        /// <param name="isRecursive"></param>
        /// <returns></returns>
        public List<Programme>? LoadProgrammes(LoadOptions? options = null, bool isRecursive = false)
        {
            options ??= new LoadOptions();

            for (int @try = 0; @try < options.MaxTries; @try++)
            {
                try
                {
                    List<Programme> programmes = new List<Programme>();

                    HtmlWeb htmlWeb = new HtmlWeb()
                    {
                        AutoDetectEncoding = false,
                        OverrideEncoding = System.Text.Encoding.UTF8
                    };

                    HtmlDocument htmlDoc = htmlWeb.Load(Url.AbsoluteUri, "GET", options.Proxy, options.Credential);

                    HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//section[@id='ProgramList']//article");

                    if (htmlNodes == null || !htmlNodes.Any())
                    {
                        if (options.DontThrowHPE)
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

                        programmes.Add(new Programme(id: programmeId, channel: this, name: programmeName)
                        {
                            LastModified = DateTime.Now
                        });
                    }

                    if (isRecursive)
                        programmes.ForEach(p => p.Episodes = p.LoadEpisodes(options));

                    return programmes;
                }
                catch (HtmlParseException)
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

            if (first.Id != second.Id)
                throw new InvalidOperationException($"Merge Error. Both side of merge must have a same {nameof(first.Id)}.");

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return newer;

            List<Programme> mergedProgrammes = newer.Programmes.Union(older.Programmes).GroupBy(programme => programme.Identity)
                .Select(programmes => Programme.Merge(programmes)).OrderBy(programme => programme.Name).ToList();

            return new Channel(id: newer.Id, name: newer.Name, programmes: mergedProgrammes)
            {
                LastModified = newer.LastModified
            };
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
                    if (config.CheckIdentity && left!.Id != right!.Id)
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
            => (Id, Programmes?.GetOrderIndependentHashCode()).GetHashCode();
        #endregion
    }
}
