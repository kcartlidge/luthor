using FluentAssertions;
using Luthor.Models;
using System.Linq;
using Xunit;

namespace Luthor.Tests
{
    public class LexerTests
    {
        private Lexer lexer;

        private void Setup(string source)
        {
            var scanner = new Scanner(source);
            lexer = new Lexer(scanner);
        }

        [Fact]
        public void NoSource_ReturnsEOF()
        {
            // Arrange.
            Setup(string.Empty);

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result.Count.Should().Be(1);
            result.First().TokenType.Should().Be(TokenTypes.EOF);
        }

        [Fact]
        public void WithVaryingLineEndings_ReturnsCorrectLineCounts()
        {
            // Arrange.
            Setup("  \r\r\n \n\r \n");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result.Last().Location.Line.Should().Be(4);
        }

        [Fact]
        public void WithVaryingText_ReturnsCorrectRuns()
        {
            // Arrange.
            Setup("Text 1234.1234\n -!ȫ");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result.Select(x => x.Location.Line).Should().BeEquivalentTo(new int[]
            {
                1,1,1,1,1,1,2,2,2,2
            });
            result.Select(x => x.Location.Column).Should().BeEquivalentTo(new int[]
            {
                1,5,6,10,11,15,1,2,4,5
            });
            result.Select(x => x.Content.ToString()).Should().BeEquivalentTo(new string[]
            {
                "Text", " ", "1234", ".", "1234","\n", " ", "-!", "ȫ", ""
            });
            result.Select(x => x.TokenType).Should().BeEquivalentTo(new TokenTypes[]
            {
                TokenTypes.Letters,
                TokenTypes.Whitespace,
                TokenTypes.Digits,
                TokenTypes.Symbols,
                TokenTypes.Digits,
                TokenTypes.EOL,
                TokenTypes.Whitespace,
                TokenTypes.Symbols,
                TokenTypes.Other,
                TokenTypes.EOF
            });
        }
    }
}
