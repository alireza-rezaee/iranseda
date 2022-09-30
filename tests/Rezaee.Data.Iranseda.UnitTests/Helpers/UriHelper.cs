namespace Rezaee.Data.Iranseda.UnitTests.Helpers
{
    internal static class UriHelper
    {
        public static Uri MakeChannelUrl(string ch)
            => new($"http://radio.iranseda.ir/live/?VALID=TRUE&ch={ch}");

        public static Uri MakeProgrammeUrl(string ch, string m)
            => new($"http://radio.iranseda.ir/Program/?VALID=TRUE&ch={ch}&m={m}");

        public static Uri MakeEpisodeUrl(string ch, string e)
            => new($"http://radio.iranseda.ir/epgarchivePart/?VALID=TRUE&ch={ch}&e={e}");
    }
}
