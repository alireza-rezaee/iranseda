using Rezaee.Data.Iranseda.Helpers;
using System;

namespace Rezaee.Data.Iranseda
{
    public class Identity
    {
        /// <summary>
        /// TODO
        /// </summary>
        public string ChannelId { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="channelId"></param>
        /// <param name="id"></param>
        public Identity(string channelId, string id)
            => (ChannelId, Id) = (channelId, id);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator ==(Identity? left, Identity? right)
            => NullHelper.NullComparison(left, right) switch
            {
                NullComparisonResult.BothNull => true,
                NullComparisonResult.OneSideOnly => false,
                NullComparisonResult.NoneNull => (left!.ChannelId, left.Id) == (right!.ChannelId, right.Id),
                _ => throw new ArgumentOutOfRangeException(),
            };

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool operator !=(Identity? left, Identity? right)
            => !(left == right);

        /// <summary>
        /// TODO
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public override bool Equals(object obj)
            => this == (Identity)obj;

        /// <summary>
        /// TODO
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
            => (ChannelId, Id).GetHashCode();
    }
}
