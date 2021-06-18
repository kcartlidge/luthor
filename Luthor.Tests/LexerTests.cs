using Luthor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Luthor.Tests
{
    [TestClass]
    public class LexerTests
    {
        private Lexer lexer;

        private void Setup(string source)
        {
            lexer = new Lexer(source);
        }

        [TestMethod]
        public void NoSource_ReturnsEOF()
        {
            // Arrange.
            Setup(string.Empty);

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            Assert.AreEqual(1, tokens.Count);
            AssertToken(tokens, 0, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithVaryingLineEndings_ReturnsCorrectLineCounts()
        {
            // Arrange.
            Setup("  \r\r\n \n\r \n");

            // Act.
            var tokens = lexer.GetTokens(true);

            // Assert.
            Assert.AreEqual(4, tokens.Last().Location.Line);
        }

        [TestMethod]
        public void WithStrings_ReturnsStringRuns()
        {
            // Arrange.
            Setup("A \"simple string\" and 'single quotes' and `back-ticks`.");

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "A");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "\"simple string\"");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "and");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "'single quotes'");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "and");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "`back-ticks`");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithStrings_FromREADME_ReturnsStringRuns()
        {
            // Arrange.
            Setup("Sample text.\nAcross 3 lines.\nWith a \"string\".");

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "Sample");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "text");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOL, "\n");
            AssertToken(tokens, ref t, TokenTypes.Letters, "Across");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Digits, "3");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "lines");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOL, "\n");
            AssertToken(tokens, ref t, TokenTypes.Letters, "With");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "a");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "\"string\"");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithUnterminatedString_ReturnsStringRun()
        {
            // Arrange.
            Setup("An 'unterminated string");

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "An");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "'unterminated string");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithString_WithLineBreak_ReturnsStringRunIncludingLineBreak()
        {
            // Arrange.
            Setup("A 'string with a \nnew line embedded'.");

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "A");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "'string with a \nnew line embedded'");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithString_WithEmbeddedStrings_ReturnsSingleStringRun()
        {
            // Arrange.
            Setup("A \"simple string `with` 'embedded' strings\".");

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "A");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "\"simple string `with` 'embedded' strings\"");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithWhitespace_RequestCompressed_ReturnsCompressed()
        {
            // Arrange.
            Setup("One two   three  \t \r four");

            // Act.
            var tokens = lexer.GetTokens(true);

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "One");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "two");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "three");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Letters, "four");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        // Asserts whether the tokens match.
        private void AssertToken(
            List<Token> tokens,
            int index,
            TokenTypes expectedTokenType,
            string expectedContent)
        {
            Assert.IsTrue(tokens.Count > index, $"Insufficient tokens (index {index}, max {tokens.Count - 1}).");

            var actual = tokens[index];
            Assert.AreEqual(expectedTokenType, actual.TokenType, $"Index: {index}.");
            Assert.AreEqual(expectedContent, actual.Content.ToString(), $"Index: {index}.");
        }

        // Asserts whether the tokens match, and increments the index ready for the next assertion.
        private void AssertToken(
            List<Token> tokens,
            ref int index,
            TokenTypes expectedTokenType,
            string expectedContent)
        {
            AssertToken(tokens, index, expectedTokenType, expectedContent);
            index += 1;
        }
    }
}
