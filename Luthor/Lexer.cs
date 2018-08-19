using Luthor.Models;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Luthor
{
    public class Lexer
    {
        private const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
        private const string digits = "0123456789";
        private const string symbols = "!£$%^&*()-_=+[]{};:'@#~,.<>/?\\|";
        private const string whitespace = " \t";
        private const string quotes = "'\"`";

        private readonly Scanner scanner;
        private int line;
        private int column;
        private char ch;

        public Lexer(Scanner scanner)
        {
            this.scanner = scanner;
            line = 1;
            column = 1;
        }

        public List<Token> GetTokens(bool compressWhitespace = false)
        {
            var tokens = new List<Token>();
            var location = new Location
            {
                Column = column,
                Line = line,
                Offset = scanner.GetCurrentPosition()
            };

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

                if (quotes.IndexOf(ch) > -1)
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
                    if (Consume(tokens, TokenTypes.Whitespace, location, whitespace))
                    {
                        if (compressWhitespace)
                        {
                            tokens.Last().Content = new StringBuilder(" ");
                        }
                        continue;
                    }
                    if (Consume(tokens, TokenTypes.Letters, location, chars)) continue;
                    if (Consume(tokens, TokenTypes.Digits, location, digits)) continue;
                    if (Consume(tokens, TokenTypes.Symbols, location, symbols)) continue;

                    // Each remaining character is a unique token.
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
