using FluentAssertions;
using Luthor;
using System.Text;
using Xunit;

namespace Luther.Tests
{
    public class ScannerTests
    {
        private const string source = "<html>\n<body>\n <h1>Hello world.</h1>\n</body>\n</html>";

        [Fact]
        public void WithEmptySource_HasNoContent()
        {
            // Arrange.
            var scanner = new Scanner(string.Empty);

            // Act.
            var hasContent = scanner.HasMore();

            // Assert.
            hasContent.Should().Be(false);
        }

        [Fact]
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
            result.ToString().Should().Be(source);
        }

        [Fact]
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
            result.HasValue.Should().Be(false);
        }

        [Fact]
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
            result.ToString().Length.Should().Be(source.Length);
            scanner.HasMore().Should().Be(false);
            scanner.EndOfSource().Should().Be(true);
        }

        [Fact]
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
            location.Should().Be(19);
        }
    }
}
