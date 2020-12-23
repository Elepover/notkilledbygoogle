using System;
using NotKilledByGoogle.Bot;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class UtilsTest
    {
        [Fact]
        public void TestNull()
            => Assert.Throws<ArgumentNullException>(() =>
            {
                Utils.ThrowIfNull<object>(null);
            });

        [Theory]
        [InlineData("Nice")]
        [InlineData(123L)]
        [InlineData(12.3)]
        [InlineData(1234)]
        [InlineData(true)]
        public void TestNotNull(object? testData)
        {
            Utils.ThrowIfNull(testData);
        }
    }
}