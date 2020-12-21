using System;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Grave;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class AnnouncementSchedulerTest
    {
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
            var gravestone = new Gravestone() {DateClose = DateTimeOffset.Now};
            scheduler.Schedule(gravestone, new AnnouncementOptions(new []{ 1, 2, 3 }));
            
            Assert.True(scheduler.IsScheduled(gravestone));
            scheduler.Cancel(gravestone);
            Assert.False(scheduler.IsScheduled(gravestone));
        }
    }
}
