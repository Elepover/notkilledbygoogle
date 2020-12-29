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
            var cts = new CancellationTokenSource();
            
            var waitTask = Assert.ThrowsAsync<TaskCanceledException>(async () => await Utils.Delay(TimeSpan.FromDays(365), cts.Token));
            cts.Cancel();
            sw.Start();
            await waitTask;

            Assert.InRange(sw.Elapsed.TotalMilliseconds, 0.0, 50.0);
        }
    }
}