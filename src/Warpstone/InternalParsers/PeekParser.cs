﻿namespace Warpstone.InternalParsers
{
    /// <summary>
    /// Parser that attempts a parse, but does not record it.
    /// </summary>
    /// <typeparam name="T">The result type of the parser.</typeparam>
    /// <seealso cref="Warpstone.Parser{T}" />
    internal class PeekParser<T> : Parser<T>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="PeekParser{T}"/> class.
        /// </summary>
        /// <param name="parser">The parser.</param>
        internal PeekParser(Parser<T> parser)
            => Parser = parser;

        /// <summary>
        /// Gets the parser.
        /// </summary>
        internal Parser<T> Parser { get; }

        /// <inheritdoc/>
        internal override ParseResult<T> Parse(string input, int position)
        {
            ParseResult<T> parseResult = Parser.Parse(input, position);

            if (parseResult.Success)
            {
                return new ParseResult<T>(default, position, position);
            }

            return new ParseResult<T>();
        }
    }
}
