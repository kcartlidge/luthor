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
            Setup("1\r\n2\r2\r\n3\n\r4\n\r5\n\n7\r\n\n9");

            // Act.
            var result = lexer.GetTokens();

            // Assert.
            result.Last().Location.Line.Should().Be(9);
        }
    }
}
