using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides methods for scheduling announcements.
    /// </summary>
    public class AnnouncementScheduler : IDisposable
    {
        private readonly Dictionary<Gravestone, CancellationTokenSource> _scheduled = new();
        private int _scheduledCount = 0;

        /// <summary>
        /// Raised when an announcement is made.
        /// </summary>
        public event EventHandler<AnnouncementEventArgs>? Announcement;

        /// <summary>
        /// Check if a <see cref="Gravestone"/> has scheduled announcements.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <returns></returns>
        public bool IsScheduled(Gravestone gravestone)
            => _scheduled.ContainsKey(gravestone);

        /// <summary>
        /// Get the count of scheduled announcements.
        /// </summary>
        public int ScheduledCount => _scheduledCount;
        
        /// <summary>
        /// Schedule an announcement and return when the announcement task is started.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="timeout">How long should the <see cref="AnnouncementScheduler"/> wait until time is up.</param>
        public async Task ScheduleAsync(Gravestone gravestone, TimeSpan timeout)
        {
            // add it to tracking list if keypair doesn't exist
            if (!_scheduled.ContainsKey(gravestone))
                _scheduled.Add(gravestone, new());
            
            // prepare the task
            var cts = _scheduled[gravestone];
            var tcs = new TaskCompletionSource();
            _ = Task.Run(async () =>
            {
                try
                {
                    Interlocked.Increment(ref _scheduledCount);
                    // ok, the task is ready, you may continue
                    tcs.SetResult();
                    await Utils.Delay(timeout, cts.Token);
                    Announcement?.Invoke(this, new (gravestone));
                }
                finally
                {
                    Interlocked.Decrement(ref _scheduledCount);
                }
            });
            await tcs.Task;
        }

        /// <inheritdoc cref="ScheduleAsync(NotKilledByGoogle.Bot.Grave.Gravestone,System.TimeSpan)"/>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="future">Time in the future when the announcement should be made.</param>
        public Task ScheduleAsync(Gravestone gravestone, DateTimeOffset future)
            => ScheduleAsync(gravestone, future - DateTimeOffset.Now);
        
        /// <summary>
        /// Automatically schedule announcements based on info provided in <see cref="Gravestone"/>.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="options">Specifies how <see cref="AnnouncementScheduler"/> should make a schedule based on <see cref="Gravestone"/> info.</param>
        public async Task<int> ScheduleAsync(Gravestone gravestone, AnnouncementOptions? options = null)
        {
            var scheduled = 0;
            options ??= AnnouncementOptions.Default;
            foreach (var futureDays in options.CriticalDays)
            {
                var estimatedFuture = gravestone.DateClose.AddDays(-futureDays);
                if (estimatedFuture <= DateTimeOffset.Now) continue;
                await ScheduleAsync(gravestone, estimatedFuture);
                scheduled++;
            }

            return scheduled;
        }

        /// <summary>
        /// Cancel all related scheduled announcements.
        /// </summary>
        /// <param name="gravestone">The <see cref="Gravestone"/> associated with corresponding announcements.</param>
        /// <param name="force">Do not throw exception when no scheduled for the <paramref name="gravestone"/> is found.</param>
        public void Cancel(Gravestone gravestone, bool force = false)
        {
            if (_scheduled.ContainsKey(gravestone))
            {
                // cancel all tasks
                _scheduled[gravestone].Cancel();
                _scheduled[gravestone].Dispose();
                // remove the pair from scheduled pool
                _scheduled.Remove(gravestone);
            }
            else if (!force) throw new InvalidOperationException("The gravestone isn't registered yet!");
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                foreach (var (_, cts) in _scheduled)
                {
                    cts.Dispose();
                }
            }
            // free native resources if there are any.
        }
    }
}
