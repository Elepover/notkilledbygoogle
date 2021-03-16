using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Statistics;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class StatsTest
    {
        #region Test configurations
        private const int RaceConditionTotalThreads = 10;
        private const int RaceConditionIncrementsPerThread = 50;
        #endregion

        #region Sample class implementation
        #pragma warning disable 8618
        [SuppressMessage("ReSharper", "UnusedMember.Local")]
        [SuppressMessage("ReSharper", "UnusedAutoPropertyAccessor.Local")]
        private class SampleStats : StatsBase
        {
            public static readonly string[] ExpectedTaggedTags =
            {
                "Total visits",
                "Total purchases"
            };

            public static readonly string[] ExpectedUntaggedTags =
            {
                "TotalIncome",
                "TotalExpense"
            };

            public const int ExpectedStatsEntries = 4;
            
            [StatName("Total visits")]
            public SafeInt TotalVisits { get; set; }
            [StatName("Total purchases")]
            public SafeInt TotalPurchases { get; set; }
            public SafeInt TotalIncome { get; set; }
            public SafeInt TotalExpense { get; set; }
        }
        #pragma warning restore 8618
        #endregion

        #region Helpers
        private void Repeat(Action action, int count)
        {
            for (int i = 0; i < count; i++)
                action();
        }
        #endregion
        
        [Fact]
        public void AttributeDiscovery()
        {
            foreach (var expectedTag in SampleStats.ExpectedTaggedTags)
                Assert.Contains(new SampleStats().RegisteredSafeInts, x => x.DisplayName == expectedTag);
        }

        [Fact]
        public void PropertiesDiscovery()
            => Assert.Equal(SampleStats.ExpectedStatsEntries, new SampleStats().RegisteredSafeInts.Count);

        [Fact]
        public void UntaggedProperties()
        {
            var stats = new SampleStats();

            foreach (var untaggedTag in SampleStats.ExpectedUntaggedTags)
                Assert.Equal(untaggedTag, stats[untaggedTag].DisplayName);
        }

        [Fact]
        public void Clearing()
        {
            var stats = new SampleStats();
            var random = new Random();

            // increase all properties' values
            foreach (var stat in stats.RegisteredSafeInts)
                stat.SafeInt.Inc(random.Next(1, 100));
            
            // make sure increment is successful
            Assert.All(stats.RegisteredSafeInts, tag => Assert.NotEqual(0, tag.SafeInt.I));

            stats.Clear();
            // make sure clear is successful
            Assert.All(stats.RegisteredSafeInts, tag => Assert.Equal(0, tag.SafeInt.I));
        }
        
        [Fact]
        public async Task RaceCondition()
        {
            var stats = new SampleStats();
            var counter = 0;
            var tcs = new TaskCompletionSource();
            
            for (var i = 0; i < RaceConditionTotalThreads; i++)
                new Thread(() =>
                {
                    Repeat(() => stats.TotalVisits.Inc(),
                        RaceConditionIncrementsPerThread);
                    
                    Interlocked.Increment(ref counter); // thread safe
                    if (counter == RaceConditionTotalThreads)
                        tcs.SetResult();
                }).Start();

            await tcs.Task;
            Assert.Equal(RaceConditionTotalThreads * RaceConditionIncrementsPerThread,
                stats.TotalVisits.I);
        }
    }
}
