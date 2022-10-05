using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Rezaee.Data.Iranseda.Configurations
{
    /// <summary>
    /// TODO
    /// </summary>
    public class LoadOptions
    {
        /// <summary>
        /// TODO
        /// </summary>
        public int MaxTries { get; set; } = 1;

        /// <summary>
        /// TODO
        /// <see cref="Exceptions.HtmlParseException"/>
        /// </summary>
        public bool DontThrowHPE { get; set; } = false;

        /// <summary>
        /// TODO
        /// </summary>
        public WebProxy? Proxy { get; set; }

        /// <summary>
        /// TODO
        /// </summary>
        public NetworkCredential? Credential { get; set; }
    }
}
