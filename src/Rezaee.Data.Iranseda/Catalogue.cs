using HtmlAgilityPack;
using Rezaee.Data.Iranseda.Configurations;
using Rezaee.Data.Iranseda.Exceptions;
using Rezaee.Data.Iranseda.Extensions;
using Rezaee.Data.Iranseda.Helpers;
using Rezaee.Data.Iranseda.JsonConverters;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using System.Web;

namespace Rezaee.Data.Iranseda
{
    /// <summary>
    /// A catalogue to store channel specifications.
    /// </summary>
    public class Catalogue : BaseCatalogue<Catalogue?>
    {
        #region Fields
        private DateTime _lastModified;
        #endregion

        #region Properties
        /// <summary>
        /// Channels in the current catalogue.
        /// </summary>
        public List<Channel>? Channels { get; set; }

        /// <inheritdoc/>
        [JsonConverter(typeof(DateTimeLocalJsonConverter))]
        public override DateTime LastModified
        {
            get
            {
                if (Channels != null && Channels.Any())
                {
                    var partitionsLastModified = Channels.OrderByDescending(c => c.LastModified).First().LastModified;
                    if (partitionsLastModified >= _lastModified)
                        return _lastModified = partitionsLastModified;
                }
                return _lastModified;
            }
            set => _lastModified = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// Create a new empty <see cref="Catalogue"/> instance.
        /// </summary>
        public Catalogue()
            => LastModified = DateTime.Now;

        /// <summary>
        /// Create a new <see cref="Catalogue"/> instance with one <paramref name="channel"/> in it.
        /// </summary>
        /// <param name="channel">The desired channel to be included in this catalogue.</param>
        public Catalogue(Channel channel)
            => (Channels, LastModified) = (new List<Channel>() { channel }, DateTime.Now);

        /// <summary>
        /// Create a new <see cref="Catalogue"/> instance with the desired <paramref name="channels"/> in it.
        /// </summary>
        /// <param name="channels">The desired channels to be included in this catalogue.</param>
        public Catalogue(List<Channel> channels)
            => (Channels, LastModified) = (channels, DateTime.Now);

        /// <summary>
        /// Create a new <see cref="Catalogue"/> instance with one <paramref name="channel"/>.
        /// and <paramref name="lastModified"/> in it.
        /// </summary>
        /// <param name="channel">The desired channel to be included in this catalogue.</param>
        /// <param name="lastModified">The date and time of the last changes applied to this catalogue.</param>
        public Catalogue(Channel channel, DateTime lastModified)
            => (Channels, LastModified) = (new List<Channel>() { channel }, lastModified);

        /// <summary>
        /// Create a new <see cref="Catalogue"/> instance with the desired <paramref name="channels"/>
        /// and <paramref name="lastModified"/> in it.
        /// </summary>
        /// <param name="channels">The desired channels to be included in this catalogue.</param>
        /// <param name="lastModified">The date and time of the last changes applied to this catalogue.</param>
        [JsonConstructor]
        public Catalogue(List<Channel> channels, DateTime lastModified)
            => (Channels, LastModified) = (channels, lastModified);
        #endregion

        #region Methods
        /// <summary>
        /// Load all the available channels from <see href="http://radio.iranseda.ir/">Iranseda</see> website.
        /// </summary>
        /// <param name="recursive">Load all the <see cref="Channel"/>s and their childs recursively.</param>
        /// <param name="tries">The number of attempts to send the request, if the previous one failed.</param>
        /// <param name="skipHtmlException">Prevent <see cref="HtmlParseException"/> from being thrown.</param>
        /// <param name="proxy">Proxy to use with the request.</param>
        /// <param name="credentials">Credentials to use when authenticating.</param>
        /// <returns>List of all the available channels.</returns>
        public List<Channel>? LoadChannels(bool recursive = false, int tries = 1, bool skipHtmlException = false,
            WebProxy? proxy = null, NetworkCredential? credentials = null)
        {
            for (int @try = 0; @try < tries; @try++)
            {
                try
                {
                    Uri channelListUri = new Uri(@"http://radio.iranseda.ir/radiolist/?VALID=TRUE");
                    List<Channel> channelList = new List<Channel>();
                    HtmlWeb htmlWeb = new HtmlWeb()
                    {
                        AutoDetectEncoding = false,
                        OverrideEncoding = System.Text.Encoding.UTF8
                    };

                    HtmlDocument htmlDoc = htmlWeb.Load(channelListUri.AbsoluteUri, "GET", proxy, credentials);

                    HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class,'ch-list')]//article");

                    if (htmlNodes == null || !htmlNodes.Any())
                    {
                        if (skipHtmlException)
                            return channelList;
                        throw new HtmlParseException("Reached incorrect HTML format or the target"
                            + $"element(s) did not exist on the page. (URL of the List of channels: {channelListUri})");
                    }

                    foreach (HtmlNode node in htmlNodes)
                    {
                        string channelId = string.Empty;
                        string channelName = node.SelectSingleNode(".//h4[contains(@class, 'ott-name')]").InnerText.Trim();

                        Uri baseUri = channelListUri;
                        string inputUrl = node.SelectSingleNode(".//a[contains(@class, 'More-items')]").Attributes["href"].Value.Trim();
                        if (Uri.TryCreate(baseUri, inputUrl, out Uri? channelUri) && channelUri != null)
                        {
                            var queryString = HttpUtility.ParseQueryString(channelUri.Query);
                            if (queryString.AllKeys.Contains("ch"))
                                channelId = queryString["ch"] ?? string.Empty;
                        }
                        else
                            throw new InvalidOperationException("Could not find one or more channel URL.");

                        channelList.Add(new Channel(id: channelId, name: channelName)
                        {
                            Catalogue = this,
                            LastModified = DateTime.Now
                        });
                    }

                    if (recursive)
                        channelList.ForEach(c => c.Programmes = c.LoadProgrammes(recursive, tries, skipHtmlException));

                    return channelList;
                }
                catch (HtmlParseException)
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// Downloads all partitions in this catalogue recursively.
        /// </summary>
        /// <param name="directory">The destination folder for saving downloaded files.</param>
        /// <param name="cataloguePath">Completed version of the current catalogue including information about downloaded files.</param>
        /// <param name="preferredMirror">The mirror server which is preferred for download.</param>
        /// <param name="skipIfExists">Ignore already downloaded and existing partitions.</param>
        /// <param name="options">Use these <see cref="JsonSerializerOptions"/> when saving the catalogue file as JSON.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public async Task DownloadAsync(string directory,
            string cataloguePath,
            Uri? preferredMirror = null,
            bool skipIfExists = true,
            JsonSerializerOptions? options = null,
            HttpClient? client = null)
        {
            var partitions = Channels.SelectMany(channel =>
                channel.Programmes.SelectMany(programme =>
                programme.Episodes.SelectMany(episode =>
                episode.Partitions)));

            if (partitions == null || !partitions.Any())
                return;

            await Partition.DownloadPartitionsAsync(
                partitions: partitions,
                directory: directory,
                skipIfExists: skipIfExists,
                preferredMirror: preferredMirror,
                client: client);

            await SaveAsync(path: cataloguePath, options);
        }

        /// <summary>
        /// Delete all downloaded files of this catalogue recursively.
        /// </summary>
        public void ClearDownloads()
        {
            if (Channels != null && Channels.Any())
                foreach (Channel channel in Channels)
                    channel.ClearDownloads();
        }

        /// <summary>
        /// Loads a <see cref="Catalogue"/> from a local file on this machine.
        /// </summary>
        /// <param name="path">The path where the catalogue file is located.</param>
        public static async Task<Catalogue?> LoadFromFileAsync(string path)
        {
            string jsonString = await File.ReadAllTextAsync(path);
            Catalogue? catalogue = Deserialize(jsonString);

            // Connect each child to its parent
            catalogue?.Channels?.ForEach(channel =>
            {
                channel.Catalogue = catalogue;
                channel.Programmes?.ForEach(programme =>
                {
                    programme.Channel = channel;
                    programme.Episodes?.ForEach(episode =>
                    {
                        episode.Programme = programme;
                        episode.Partitions?.ForEach(partition => partition.Episode = episode);
                    });
                });
            });

            return catalogue;
        }

        /// <summary>
        /// It loads a <see cref="Catalogue"/> from an internet file.
        /// </summary>
        /// <param name="url">The URL where the catalogue file is located.</param>
        /// <param name="tries">The number of attempts to send the request, if the previous one failed.</param>
        /// <param name="client">Use to pass the predefined <see cref="HttpClient"/>.</param>
        public static async Task<Catalogue?> LoadFromRemoteAsync(Uri url, int tries = 1,
            HttpClient? client = null)
        {
            Catalogue? catalogue;

            for (int @try = 0; @try < tries; @try++)
            {
                try
                {
                    client ??= new HttpClient();
                    client.DefaultRequestHeaders.Accept.Clear();
                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();


                    using (var reponseStream = await response.Content.ReadAsStreamAsync())
                    {
                        var options = new JsonSerializerOptions
                        {
                            PropertyNameCaseInsensitive = true,
                        };
                        catalogue = await JsonSerializer.DeserializeAsync<Catalogue>(reponseStream, options);
                    }

                    // Connect each child to its parent
                    catalogue?.Channels?.ForEach(channel =>
                    {
                        channel.Catalogue = catalogue;
                        channel.Programmes?.ForEach(programme =>
                        {
                            programme.Channel = channel;
                            programme.Episodes?.ForEach(episode =>
                            {
                                episode.Programme = programme;
                                episode.Partitions?.ForEach(partition => partition.Episode = episode);
                            });
                        });
                    });

                    return catalogue;
                }
                catch (Exception)
                {
                    continue;
                }
            }

            return null;
        }

        /// <summary>
        /// This will save the current <see cref="Catalogue"/> as a file locally.
        /// </summary>
        /// <param name="path">The full name of the file to save this catalogue to.</param>
        /// <param name="options">Use these <see cref="JsonSerializerOptions">options</see> when saving the catalogue file as JSON.</param>
        public async Task SaveAsync(string path, JsonSerializerOptions? options = null)
            => await File.WriteAllTextAsync(path, Serialize(this, options));

        /// <summary>
        /// Merges the contents of several <see cref="Catalogue"/>s together.
        /// </summary>
        /// <param name="catalogues">The desired <see cref="Catalogue"/>s for merge.</param>
        /// <returns>Returns a new <see cref="Catalogue"/> as the result of the merge process.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static Catalogue Merge(IEnumerable<Catalogue> catalogues)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(catalogues, nameof(catalogues));

            if (!catalogues.Any())
                throw new ArgumentException(message: "No element is specified for merge.", paramName: nameof(catalogues));

            if (catalogues.Count() == 1)
                return catalogues.First();
            else if (catalogues.Count() == 2)
                return Merge(catalogues.ElementAt(0), catalogues.ElementAt(1));

            Catalogue merged = catalogues.First();
            foreach (var catalogue in catalogues.Skip(1))
                merged = Merge(merged, catalogue);
            return merged;
        }

        /// <summary>
        /// Merges the contents of two <see cref="Catalogue"/>s together.
        /// </summary>
        /// <param name="first">First item to merge.</param>
        /// <param name="second">Second item to merge.</param>
        /// <returns>Returns a new <see cref="Catalogue"/> as the result of the merge process.</returns>
        public static Catalogue Merge(Catalogue first, Catalogue second)
        {
            ThrowHelper.ThrowArgumentNullExceptionIfNull(first, nameof(first));
            ThrowHelper.ThrowArgumentNullExceptionIfNull(second, nameof(second));

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return older;

            List<Channel> mergedChannels = newer.Channels.Union(older.Channels).GroupBy(channel => channel.Id)
                .Select(channels => Channel.Merge(channels)).OrderBy(channel => channel.Name).ToList();

            return new Catalogue(channels: mergedChannels, lastModified: newer.LastModified);
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Catalogue"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <param name="config">Apply equality configuration.</param>
        /// <returns>Are they equal?</returns>
        /// <exception cref="ArgumentOutOfRangeException"></exception>
        public static bool Equals(Catalogue? left, Catalogue? right, CataloguesEqualityConfiguration config)
        {
            switch (NullHelper.NullComparison(left, right))
            {
                case NullComparisonResult.BothNull:
                    return true;
                case NullComparisonResult.OneSideOnly:
                    return false;
                case NullComparisonResult.NoneNull:
                    if (config.CheckChannels)
                        switch (NullHelper.NullComparison(left!.Channels, right!.Channels))
                        {
                            case NullComparisonResult.BothNull:
                                break;
                            case NullComparisonResult.OneSideOnly:
                                return false;
                            case NullComparisonResult.NoneNull:
                                if (left.Channels.Distinct().Count() != right.Channels.Distinct().Count())
                                    return false;

                                if (left.Channels.Any(le => !right.Channels.Any(re => Channel.Equals(le, re, config.ChannelsConfig)))
                                || right.Channels.Any(re => !left.Channels.Any(le => Channel.Equals(le, re, config.ChannelsConfig))))
                                    return false;
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    return true;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        /// <summary>
        /// It is used to compare the equality of two <see cref="Catalogue"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they equal?</returns>
        public static bool operator ==(Catalogue? left, Catalogue? right)
            => Equals(left, right, new CataloguesEqualityConfiguration());

        /// <summary>
        /// It is used to compare the inequality of two <see cref="Catalogue"/>s.
        /// </summary>
        /// <param name="left">The left operand of comparison.</param>
        /// <param name="right">The right operand of comparison.</param>
        /// <returns>Are they not equal?</returns>
        public static bool operator !=(Catalogue? left, Catalogue? right)
            => !(left == right);

        /// <inheritdoc/>
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
                return false;

            return this == (Catalogue)obj;
        }

        /// <inheritdoc/>
        public override int GetHashCode()
            => Channels?.GetOrderIndependentHashCode() ?? 0;
        #endregion
    }
}
