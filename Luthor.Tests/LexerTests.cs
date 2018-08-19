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

        [Fact]
        public void WithStrings_ReturnsStringRuns()
        {
            // Arrange.
            Setup("A \"simple string\" and 'single quotes' and `back-ticks`.");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result[2].Content.ToString().Should().Be("\"simple string\"");
            result[6].Content.ToString().Should().Be("'single quotes'");
            result[10].Content.ToString().Should().Be("`back-ticks`");
            result[2].TokenType.Should().Be(TokenTypes.String);
            result[6].TokenType.Should().Be(TokenTypes.String);
            result[10].TokenType.Should().Be(TokenTypes.String);
        }

        [Fact]
        public void WithUnterminatedString_ReturnsStringRun()
        {
            // Arrange.
            Setup("An 'unterminated string");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result[2].Content.ToString().Should().Be("'unterminated string");
            result[2].TokenType.Should().Be(TokenTypes.String);
        }

        [Fact]
        public void WithString_WithLineBreak_ReturnsStringRunIncludingLineBreak()
        {
            // Arrange.
            Setup("A 'string with a \nnew line embedded'.");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result[2].Content.ToString().Should().Be("'string with a \nnew line embedded'");
            result[2].TokenType.Should().Be(TokenTypes.String);
        }

        [Fact]
        public void WithString_WithEmbeddedStrings_ReturnsSingleStringRun()
        {
            // Arrange.
            Setup("A \"simple string `with` 'embedded' strings\".");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result[2].Content.ToString().Should().Be("\"simple string `with` 'embedded' strings\"");
            result[2].TokenType.Should().Be(TokenTypes.String);
        }

        [Fact]
        public void WithWhitespace_RequestCompressed_ReturnsCompressed()
        {
            // Arrange.
            Setup("One two   three  \t \r four");

            // Act.
            var result = lexer.GetTokens(true);

            // Assert.
            result[1].Content.ToString().Should().Be(" ");
            result[3].Content.ToString().Should().Be(" ");
            result[5].Content.ToString().Should().Be(" ");
            result[6].Content.ToString().Should().Be("four");
            result[1].TokenType.Should().Be(TokenTypes.Whitespace);
            result[3].TokenType.Should().Be(TokenTypes.Whitespace);
            result[5].TokenType.Should().Be(TokenTypes.Whitespace);
            result[6].TokenType.Should().Be(TokenTypes.Letters);
        }
    }
}
