using Luthor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luthor
{
    /// <summary>
    /// Instances of this class convert text to a sequence of tokens.
    /// </summary>
    public class Lexer
    {
        /// <summary>
        /// The collection of characters representing standard letters (eg A, B).
        /// </summary>
        public string Chars { get; set; }

        /// <summary>
        /// The collection of characters representing standard digits (eg 0, 1).
        /// </summary>
        public string Digits { get; set; }

        /// <summary>
        /// The collection of characters representing symbols (eg !, +).
        /// </summary>
        public string Symbols { get; set; }

        /// <summary>
        /// The collection of characters representing whitespace (eg space, tab).
        /// </summary>
        public string Whitespace { get; set; }

        /// <summary>
        /// The collection of characters representing quote terminators (eg ", ').
        /// </summary>
        public string Quotes { get; set; }

        private readonly Scanner scanner;
        private int line;
        private int column;
        private char ch;

        /// <summary>
        /// Create a lexer for converting text to a sequence of tokens.
        /// </summary>
        /// <param name="source">
        /// Text content to convert.
        /// </param>
        public Lexer(string source)
        {
            Chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Digits = "0123456789";
            Symbols = "!£$%^&*()-_=+[]{};:'@#~,.<>/?\\|";
            Whitespace = " \t";
            Quotes = "'\"`";

            scanner = new Scanner(source);
            line = 1;
            column = 1;
        }

        /// <summary>
        /// Scans the source text and converts it to a sequence of tokens.
        /// </summary>
        /// <param name="compressWhitespace">
        /// If true, runs of consecutive whitespace are compressed to single spaces.
        /// </param>
        /// <returns>
        /// A collection of tokens representing the source text.
        /// </returns>
        public List<Token> GetTokens(bool compressWhitespace = false)
        {
            var tokens = new List<Token>();

            Location location;
            Token lastToken;
            while (scanner.HasMore())
            {
                // Consume one char and set the location.
                ch = scanner.GetNext().Value;
                location = new Location
                {
                    Column = column,
                    Line = line,
                    Offset = scanner.GetCurrentPosition()
                };
                column += 1;

                if (Quotes.IndexOf(ch) > -1)
                {
                    // Start string, read all until the next *matching* string token.
                    var token = Token.Create(tokens, location, TokenTypes.String, ch);
                    ConsumeUntil(token, ch);
                    continue;
                }
                else if (ch == (char)13)
                {
                    // Ignore carriage returns.
                    continue;
                }
                else if (ch == (char)10)
                {
                    // Each line feed is a unique token.
                    Token.Create(tokens, location, TokenTypes.EOL, ch);
                    line += 1;
                    column = 1;
                }
                else
                {
                    // Consume specific types.
                    if (Consume(tokens, TokenTypes.Whitespace, location, Whitespace))
                    {
                        if (compressWhitespace)
                        {
                            tokens.Last().Content = new StringBuilder(" ");
                        }
                        continue;
                    }
                    if (Consume(tokens, TokenTypes.Letters, location, Chars)) continue;
                    if (Consume(tokens, TokenTypes.Digits, location, Digits)) continue;
                    if (Consume(tokens, TokenTypes.Symbols, location, Symbols)) continue;

                    // Remaining characters form a run of 'Other' tokens.
                    lastToken = tokens.Last();
                    if (lastToken.TokenType == TokenTypes.Other)
                        lastToken.Content.Append(ch);
                    else
                        Token.Create(tokens, location, TokenTypes.Other, ch);
                }
            }

            // Always have an end-of-file token.
            location = new Location
            {
                Column = column,
                Line = line,
                Offset = scanner.GetCurrentPosition()
            };
            Token.Create(tokens, location, TokenTypes.EOF, string.Empty);
            return tokens;
        }

        /// <summary>
        /// Scans the source text and converts it to a sequence of tokens.
        /// This is returned split as a list of numbered lines.
        /// </summary>
        /// <param name="compressWhitespace">
        /// If true, runs of consecutive whitespace are compressed to single spaces.
        /// </param>
        /// <returns>
        /// A collection of tokens representing the source text,
        /// split as a list of numbered lines.
        /// </returns>
        public SortedList<int, List<Token>> GetTokensAsLines(bool compressWhitespace = false)
        {
            var tokens = GetTokens(compressWhitespace);
            var lines = new SortedList<int, List<Token>>();

            var lastLineNumber = -1;
            foreach (var token in tokens)
            {
                var thisLineNumber = token.Location.Line;
                if (thisLineNumber == lastLineNumber) lines[thisLineNumber].Add(token);
                else lines[thisLineNumber] = new List<Token> { token };
                lastLineNumber = thisLineNumber;
            }
            return lines;
        }

        private bool Consume(List<Token> tokens, TokenTypes tokenType, Location location, string charset)
        {
            // If we're done (other than \r), exit and flag nothing happened.
            if (charset.IndexOf(ch) == -1 && ch != (char)13) return false;

            // Start consuming.
            var token = Token.Create(tokens, location, tokenType, ch);
            while (scanner.HasMore())
            {
                // Repeat as long as we're not about to hit something else.
                var next = scanner.PeekNext();
                if (charset.IndexOf(next.Value) == -1 && next.Value != (char)13) break;

                token.Content.Append(scanner.GetNext().Value);
                column += 1;
            }
            return true;
        }

        private void ConsumeUntil(Token token, char endChar)
        {
            while (scanner.HasMore())
            {
                // Repeat until we hit the end char.
                var next = scanner.GetNext();
                token.Content.Append(next.Value);
                column += 1;
                if (endChar == next.Value)
                {
                    break;
                }
            }
        }
    }
}
