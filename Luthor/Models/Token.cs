using System.Collections.Generic;
using System.Text;

namespace Luthor.Models
{
    public class Token
    {
        public TokenTypes TokenType;
        public Location Location;
        public StringBuilder Content;

        public static Token Create(
            List<Token> tokens,
            Location location,
            TokenTypes tokenType,
            char currentChar
        )
        {
            return Create(tokens, location, tokenType, currentChar.ToString());
        }

        public static Token Create(
            List<Token> tokens,
            Location location,
            TokenTypes tokenType,
            string newContent
        )
        {
            var t = new Token
            {
                Location = location,
                TokenType = tokenType,
                Content = new StringBuilder(newContent),
            };
            tokens.Add(t);
            return t;
        }

        public override string ToString()
        {
            return string.Format(
                "{0,4},{1,-3} {2,-10}   {3}",
                Location.Line,
                Location.Column,
                TokenType,
                Content
            ).TrimEnd();
        }
    }
}
