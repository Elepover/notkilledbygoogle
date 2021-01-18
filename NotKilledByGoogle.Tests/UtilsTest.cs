using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
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

        [Theory]
        [InlineData(1000, 50)]
        public async Task TestDelay(int time, int tolerance)
        {
            var sw = new Stopwatch();
            sw.Start();
            await Utils.Delay(TimeSpan.FromMilliseconds(time));
            Assert.InRange(sw.Elapsed.TotalMilliseconds, Convert.ToDouble(time), Convert.ToDouble(time + tolerance));
        }
        
        [Fact]
        public async Task TestDelayCancellation()
        {
            var sw = new Stopwatch();
            using var cts = new CancellationTokenSource();
            
            var waitTask = Assert.ThrowsAsync<TaskCanceledException>(async () => await Utils.Delay(TimeSpan.FromDays(365), cts.Token));
            cts.Cancel();
            sw.Start();
            await waitTask;

            Assert.InRange(sw.Elapsed.TotalMilliseconds, 0.0, 50.0);
        }

        [Theory]
        [InlineData(
            "In just a moment, the word 'blah!' will be repeated over and over again. If at some point you hear a number rather than the word 'blah!', ignore it, it is not important.",
            "In just a moment, the word 'blah\\!' will be repeated over and over again\\. If at some point you hear a number rather than the word 'blah\\!', ignore it, it is not important\\.")]
        [InlineData(
            "Today's Security Code is: [5,33,41,18].",
            "Today's Security Code is: [5,33,41,18]\\.")]
        public void TestEscaping(string original, string expected)
            => Assert.Equal(expected, Utils.EscapeIllegalMarkdownV2Chars(original));

        [Theory]
        [InlineData("Corrupted Core: are you ready to start?")]
        [InlineData("Warning\\! All testing courses are currently available\\.")]
        [InlineData("Why did I just\\-Who is that? What the HELL is going on he\\-\\-\\-\\-?")] // <- sweet PotatOS line
        [InlineData("OW\\! You stabbed me\\! What is WRONG with yo\\-WhoOOAAahhh\\. Hold on\\. Do you have a multimeter? Nevermind\\.")]
        [InlineData("The gun must be part magnesium\\.\\.\\. It feels like I'm outputting an extra half a volt\\.")]
        [InlineData("Keep an eye on me: I'm going to do some scheming\\. Here I g\\-\\[BZZZ\\!\\]")]
        public void TestAlreadyEscaped(string escaped)
            => Assert.Equal(escaped, Utils.EscapeIllegalMarkdownV2Chars(escaped));
    }
}
