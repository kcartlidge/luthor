using Luthor;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Text;

namespace Luther.Tests
{
    [TestClass]
    public class ScannerTests
    {
        private const string source = "<html>\n<body>\n <h1>Hello world.</h1>\n</body>\n</html>";

        [TestMethod]
        public void WithEmptySource_HasNoContent()
        {
            // Arrange.
            var scanner = new Scanner(string.Empty);

            // Act.
            var hasContent = scanner.HasMore();

            // Assert.
            Assert.IsFalse(hasContent);
        }

        [TestMethod]
        public void WithSource_HasSource()
        {
            // Arrange.
            var scanner = new Scanner(source);
            var result = new StringBuilder();

            // Act.
            while (scanner.HasMore())
            {
                result.Append(scanner.GetNext());
            }

            // Assert.
            Assert.AreEqual(result.ToString(), source);
        }

        [TestMethod]
        public void GetNext_PassedEnd_ReturnsNull()
        {
            // Arrange.
            var scanner = new Scanner(source);

            // Act.
            while (scanner.HasMore())
            {
                scanner.GetNext();
            }
            var result = scanner.GetNext();

            // Assert.
            Assert.IsFalse(result.HasValue);
        }

        [TestMethod]
        public void GetNext_HasNoMore_EndOfSourceIsTrue()
        {
            // Arrange.
            var scanner = new Scanner(source);
            var result = new StringBuilder();

            // Act.
            while (scanner.HasMore())
            {
                result.Append(scanner.GetNext());
            }

            // Assert.
            Assert.AreEqual(source.Length, result.Length);
            Assert.IsFalse(scanner.HasMore());
            Assert.IsTrue(scanner.EndOfSource());
        }

        [TestMethod]
        public void WithSource_CanPeek()
        {
            // Arrange.
            var scanner = new Scanner(source);

            // Act.
            var ch = scanner.PeekNext();

            // Assert.
            Assert.AreEqual(source[0], ch.Value);
        }

        [TestMethod]
        public void GetNext_PassedEnd_PeekReturnsNull()
        {
            // Arrange.
            var scanner = new Scanner(source);

            // Act.
            while (scanner.HasMore())
            {
                scanner.GetNext();
            }
            var result = scanner.PeekNext();

            // Assert.
            Assert.IsFalse(result.HasValue);
        }

        [TestMethod]
        public void GetCurrentPosition_GetsOffset()
        {
            // Arrange.
            var source = "Line 1\nLine 2\nLine 3";
            var scanner = new Scanner(source);

            // Act.
            for (var i = 1; i < source.Length; i++)
            {
                scanner.GetNext();
            }
            var location = scanner.GetCurrentPosition();

            // Assert.
            Assert.AreEqual(19, location);
        }
    }
}
