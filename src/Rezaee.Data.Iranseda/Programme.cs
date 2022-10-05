﻿using HtmlAgilityPack;
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
        public (string ChannelId, string ProgrammeId) Id { get; set; }

        /// <summary>
        /// The name of the current programme.
        /// </summary>
        public string? Name { get; set; }

        /// <summary>
        /// The URL of the current programme.
        /// </summary>
        public Uri Url { get => UrlHelper.MakeProgrammeUrl(ch: Id.ChannelId, m: Id.ProgrammeId); }

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
            get => _channel ?? new Channel(id: Id.ChannelId);
            set => _channel = value;
        }
        #endregion

        #region Constructors
        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="identity"></param>
        /// <param name="name"></param>
        /// <param name="episodes"></param>
        public Programme((string channelId, string programmeId) identity,
            string? name = null,
            List<Episode>? episodes = null)
            => (Id, Name, Episodes) = ((ChannelId: identity.programmeId, ProgrammeId: identity.channelId), name, episodes);

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
            => (Id, Channel, Name, Episodes) = ((ChannelId: channel.Id, ProgrammeId: id), channel, name, episodes);
        #endregion

        #region Methods
        /// <summary>
        /// Load all the episodes of this programme from <see href="http://radio.iranseda.ir/">Iranseda</see> website.
        /// </summary>
        /// <param name="recursive">Load all the <see cref="Episode"/>s and their childs recursively.</param>
        /// <param name="tries">The number of attempts to send the request, if the previous one failed.</param>
        /// <param name="skipHtmlException">Prevent <see cref="HtmlParseException"/> from being thrown.</param>
        /// <param name="proxy">Proxy to use with the request.</param>
        /// <param name="credentials">Credentials to use when authenticating.</param>
        /// <returns>List of all the available episodes for the current programme.</returns>
        public List<Episode>? LoadEpisodes(bool recursive = false, int tries = 1, bool skipHtmlException = false,
            WebProxy? proxy = null, NetworkCredential? credentials = null)
        {
            for (int @try = 0; @try < tries; @try++)
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

                    HtmlDocument htmlDoc = htmlWeb.Load(Url.AbsoluteUri, "GET", proxy, credentials);

                    HtmlNodeCollection htmlNodes = htmlDoc.DocumentNode.SelectNodes("//div[contains(@class, 'episode-box')]//li");

                    if (htmlNodes == null || !htmlNodes.Any())
                    {
                        if (skipHtmlException)
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

                    if (recursive)
                        episodes.ForEach(e => e.Partitions = e.LoadPartitions(tries, skipHtmlException));

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

            if (first.Id != second.Id)
                throw new InvalidOperationException($"Merge Error. Both side of merge must have a same {nameof(first.Id)}.");

            (var newer, var older) = first.LastModified >= second.LastModified ? (first, second) : (second, first);

            if (newer == older)
                return newer;

            List<Episode> mergedEpisodes = newer.Episodes.Union(older.Episodes).GroupBy(episode => episode.Id)
                .Select(episodes => Episode.Merge(episodes)).OrderByDescending(episode => episode.Date).ToList();

            return new Programme(identity: newer.Id, name: newer.Name, episodes: mergedEpisodes)
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
                    if (config.CheckIdentity && left!.Id != right!.Id)
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
            => (Id, Episodes?.GetOrderIndependentHashCode()).GetHashCode();
        #endregion
    }
}
