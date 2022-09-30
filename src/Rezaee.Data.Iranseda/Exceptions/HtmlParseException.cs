using System;

namespace Rezaee.Data.Iranseda.Exceptions
{
    /// <summary>
    /// The exception that is thrown when an error occurred during parsing HTML.
    /// </summary>
    public class HtmlParseException : Exception
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlParseException"/> class.
        /// </summary>
        public HtmlParseException()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlParseException"/> class with a specified error message.
        /// </summary>
        /// <param name="message">A message that describes the error.</param>
        public HtmlParseException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HtmlParseException"/> class with a specified
        /// error message and the exception that is the cause of this exception.
        /// </summary>
        /// <param name="message">The error message that explains the reason for this exception.</param>
        /// <param name="inner">
        /// The exception that is the cause of the current exception, or a null reference
        /// (Nothing in Visual Basic) if no inner exception is specified.
        /// </param>
        public HtmlParseException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
