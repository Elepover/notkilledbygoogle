using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides methods for scheduling announcements.
    /// </summary>
    public class AnnouncementScheduler : IDisposable
    {
        private readonly Dictionary<Gravestone, GraveAnnouncementCollection> _scheduled = new();

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
        public int ScheduledCount {
            get
            {
                var result = 0;
                foreach (var (_, collection) in _scheduled)
                {
                    result += collection.AnnouncementTasks.Count;
                }

                return result;
            }
        }

        /// <summary>
        /// Get scheduled announcements' designated fire-off time.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <exception cref="KeyNotFoundException">The <see cref="Gravestone"/> isn't registered.</exception>
        /// <returns></returns>
        public DateTimeOffset[] GetAnnouncementDates(Gravestone gravestone)
            => _scheduled[gravestone].AnnouncementTasks.Select(x => x.Item1).ToArray();

        /// <summary>
        /// Get scheduled gravestones.
        /// </summary>
        /// <returns></returns>
        public ImmutableList<Gravestone> GetGravestones()
            => _scheduled.Keys.ToImmutableList();

        /// <summary>
        /// Schedule an announcement and return when the announcement task is started.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="timeout">How long should the <see cref="AnnouncementScheduler"/> wait until time is up.</param>
        public Task ScheduleAsync(Gravestone gravestone, TimeSpan timeout)
            => ScheduleAsync(gravestone, DateTimeOffset.Now + timeout);

        /// <inheritdoc cref="ScheduleAsync(NotKilledByGoogle.Bot.Grave.Gravestone,System.TimeSpan)"/>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="future">Time in the future when the announcement should be made.</param>
        public async Task ScheduleAsync(Gravestone gravestone, DateTimeOffset future)
        {
            // add it to tracking list if keypair doesn't exist
            if (!_scheduled.ContainsKey(gravestone))
                _scheduled.Add(gravestone, new());

            // prepare the task
            var taskCollection = _scheduled[gravestone];
            var tcs = new TaskCompletionSource();
            var task = Task.Run(async () =>
            {
                tcs.SetResult();
                await Utils.Delay(future - DateTimeOffset.Now, taskCollection.CancellationTokenSource.Token);
                Announcement?.Invoke(this, new(gravestone));
            });
            taskCollection.AnnouncementTasks.Add((future, task));
            await tcs.Task;
        }
        
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
                // cancel all tasks (by disposing the CancellationTokenSource themselves)
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
                foreach (var (_, announcementCollection) in _scheduled)
                {
                    announcementCollection.Dispose();
                }
            }
            // free native resources if there are any.
        }
    }
}
