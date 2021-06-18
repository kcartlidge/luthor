using Luthor.Models;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Linq;

namespace Luthor.Tests
{
    [TestClass]
    public class LexerTests
    {
        [TestMethod]
        public void NoSource_ReturnsEOF()
        {
            // Arrange.
            var lexer = new Lexer(string.Empty);

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
            var lexer = new Lexer("  \r\r\n \n\r \n");

            // Act.
            var tokens = lexer.GetTokens(true);

            // Assert.
            Assert.AreEqual(4, tokens.Last().Location.Line);
        }

        [TestMethod]
        public void WithOneOfEachKindOfToken_ReturnsCorrectTokens()
        {
            // Arrange.
            var lexer = new Lexer("AB 01 +- 'AB' àë \n");

            // Act.
            var tokens = lexer.GetTokens(true);

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "AB");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Digits, "01");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Symbols, "+-");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "'AB'");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.Other, "àë");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, " ");
            AssertToken(tokens, ref t, TokenTypes.EOL, "\n");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithStrings_ReturnsStringRuns()
        {
            // Arrange.
            var lexer = new Lexer("A \"simple string\" and 'single quotes' and `back-ticks`.");

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
            var lexer = new Lexer("Sample text.\nAcross 3 lines.\nWith a \"multi 'word' string\".");

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
            AssertToken(tokens, ref t, TokenTypes.String, "\"multi 'word' string\"");
            AssertToken(tokens, ref t, TokenTypes.Symbols, ".");
            AssertToken(tokens, ref t, TokenTypes.EOF, string.Empty);
        }

        [TestMethod]
        public void WithUnterminatedString_ReturnsStringRun()
        {
            // Arrange.
            var lexer = new Lexer("An 'unterminated string");

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
            var lexer = new Lexer("A 'string with a \nnew line embedded'.");

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
            var lexer = new Lexer("A \"simple string `with` 'embedded' strings\".");

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
            var lexer = new Lexer("One two   three  \t \r four");

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

        [TestMethod]
        public void WithCustomTokenCharacters_ReturnsCorrectTokens()
        {
            // Arrange.
            var lexer = new Lexer("11 22 3--3 44 555 ëë \n")
            {
                Chars = "1",
                Digits = "2",
                Quotes = "3",      // "--" is written as 3--3
                Symbols = "4",
                Whitespace = "5",  // Three spaces are written as 555
            };

            // Act.
            var tokens = lexer.GetTokens();

            // Assert.
            var t = 0;
            AssertToken(tokens, ref t, TokenTypes.Letters, "11");
            AssertToken(tokens, ref t, TokenTypes.Other, " ");
            AssertToken(tokens, ref t, TokenTypes.Digits , "22");
            AssertToken(tokens, ref t, TokenTypes.Other, " ");
            AssertToken(tokens, ref t, TokenTypes.String, "3--3");
            AssertToken(tokens, ref t, TokenTypes.Other, " ");
            AssertToken(tokens, ref t, TokenTypes.Symbols, "44");
            AssertToken(tokens, ref t, TokenTypes.Other, " ");
            AssertToken(tokens, ref t, TokenTypes.Whitespace, "555");
            AssertToken(tokens, ref t, TokenTypes.Other, " ëë ");
            AssertToken(tokens, ref t, TokenTypes.EOL, "\n");
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
