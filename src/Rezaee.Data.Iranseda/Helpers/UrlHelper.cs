using System;
using System.Collections.Generic;
using System.Text;

namespace Rezaee.Data.Iranseda.Helpers
{
    public static class UrlHelper
    {
        public static Uri MakeChannelUrl(string ch)
            => new Uri($"http://radio.iranseda.ir/live/?VALID=TRUE&ch={ch}");

        public static Uri MakeProgrammeUrl(string ch, string m)
            => new Uri($"http://radio.iranseda.ir/Program/?VALID=TRUE&ch={ch}&m={m}");

        public static Uri MakeEpisodeUrl(string ch, string e)
            => new Uri($"http://radio.iranseda.ir/epgarchivePart/?VALID=TRUE&ch={ch}&e={e}");
    }
}
