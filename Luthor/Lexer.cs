using Luthor.Models;
using System.Collections.Generic;

namespace Luthor
{
    public class Lexer
    {
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

        public List<Token> GetTokens()
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
                ch = scanner.GetNext().Value;
                location = new Location
                {
                    Column = column,
                    Line = line,
                    Offset = scanner.GetCurrentPosition()
                };

                if (ch == (char)13) continue;
                if (ch == (char)10)
                {
                    Token.Create(tokens, location, TokenTypes.EOL, ch);
                    line += 1;
                    column = 1;
                    continue;
                }
            }

            Token.Create(tokens, location, TokenTypes.EOF, string.Empty);
            return tokens;
        }
    }
}
