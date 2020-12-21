using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Grave;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class AnnouncementSchedulerTest
    {
        public class DateTimeOffsetWrapper
        {
            public DateTimeOffsetWrapper(DateTimeOffset dateTimeOffset)
            {
                DateTimeOffset = dateTimeOffset;
            }
            public DateTimeOffset DateTimeOffset { get; }
        }
        
        private static IEnumerable<DateTimeOffsetWrapper[]> PastDueAnnouncementDates()
        {
            yield return new DateTimeOffsetWrapper[] { new(DateTimeOffset.Now.AddSeconds(-1)) };
            yield return new DateTimeOffsetWrapper[] { new(DateTimeOffset.Now.AddMinutes(-1)) };
            yield return new DateTimeOffsetWrapper[] { new(DateTimeOffset.Now.AddHours(-1)) };
            yield return new DateTimeOffsetWrapper[] { new(DateTimeOffset.Now.AddDays(-1)) };
            yield return new DateTimeOffsetWrapper[] { new(DateTimeOffset.Now.AddMonths(-1)) };
        }
        
        [Fact]
        public async Task AnnouncementScheduling()
        {
            var scheduler = new AnnouncementScheduler();
            var receivedEventTask = Assert.RaisesAsync<AnnouncementEventArgs>(
                x => scheduler.Announcement += x,
                x => scheduler.Announcement -= x,
                async () =>
                {
                    var gravestone = new Gravestone() {DateClose = DateTimeOffset.Now.AddMilliseconds(100)};
                    scheduler.Schedule(gravestone, AnnouncementOptions.Default);
                    // counter the overheads of scheduling
                    await Task.Delay(250);
                });
            var receivedEventAsync = await receivedEventTask;
            Assert.NotNull(receivedEventAsync);
        }
        
        [Fact]
        public async Task AnnouncementMultiScheduling()
        {
            const int expectedAnnouncements = 3;
            var count = 0;
            var scheduler = new AnnouncementScheduler();
            var gravestone = new Gravestone() {DateClose = DateTimeOffset.Now};
            scheduler.Announcement += (_, args) =>
            {
                if (args.Gravestone == gravestone)
                    count++;
            };
            
            for (int expected = expectedAnnouncements; expected >= 1; expected--)
            {
                scheduler.Schedule(gravestone, TimeSpan.FromMilliseconds(expected * 100));   
            }
            
            // counter performance overhead to ensure better success possibility
            await Task.Delay(TimeSpan.FromMilliseconds((expectedAnnouncements + 1) * 100));
            
            Assert.Equal(expectedAnnouncements, count);
        }

        [Fact]
        public void AnnouncementCancel()
        {
            var scheduler = new AnnouncementScheduler();
            var gravestone = new Gravestone() {DateClose = DateTimeOffset.Now.AddMonths(1)};
            scheduler.Schedule(gravestone, new AnnouncementOptions(new []{ 1, 2, 3 }));
            
            Assert.True(scheduler.IsScheduled(gravestone));
            scheduler.Cancel(gravestone);
            Assert.False(scheduler.IsScheduled(gravestone));
        }

        [Fact]
        public void AnnouncementNow()
        {
            var scheduler = new AnnouncementScheduler();
            var gravestone = new Gravestone() {DateClose = DateTimeOffset.Now};
            scheduler.Schedule(gravestone, AnnouncementOptions.Default);
            
            Assert.False(scheduler.IsScheduled(gravestone));
        }

        [Theory]
        [MemberData(nameof(PastDueAnnouncementDates))]
        public void AnnouncementPastDue(DateTimeOffsetWrapper expected)
        {
            var scheduler = new AnnouncementScheduler();
            var gravestone = new Gravestone() {DateClose = expected.DateTimeOffset};
            scheduler.Schedule(gravestone, AnnouncementOptions.Default);
            
            Assert.False(scheduler.IsScheduled(gravestone));
        }
    }
}
