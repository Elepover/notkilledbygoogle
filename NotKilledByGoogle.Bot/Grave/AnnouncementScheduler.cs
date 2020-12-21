using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides methods for scheduling announcements.
    /// </summary>
    public class AnnouncementScheduler
    {
        private const int MaxRetryAttempts = 3;
        private readonly Dictionary<Gravestone, CancellationTokenSource> _scheduled = new();

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
        /// Schedule an announcement.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="timeout">How long should the <see cref="AnnouncementScheduler"/> wait until time is up.</param>
        public void Schedule(Gravestone gravestone, TimeSpan timeout)
        {
            // add it to tracking list if keypair doesn't exist
            if (!_scheduled.ContainsKey(gravestone))
                _scheduled.Add(gravestone, new());
            
            // prepare the task
            var cts = _scheduled[gravestone];
            _ = Task.Run(async () =>
            {
                // doesn't need to catch TaskCanceledException: it's inside the task
                await Task.Delay(timeout, cts.Token);
                Announcement?.Invoke(this, new (gravestone));
            });
        }

        /// <inheritdoc cref="Schedule(NotKilledByGoogle.Bot.Grave.Gravestone,System.TimeSpan)"/>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="future">Time in the future when the announcement should be made.</param>
        public void Schedule(Gravestone gravestone, DateTimeOffset future)
            => Schedule(gravestone, future - DateTimeOffset.Now);
        
        /// <summary>
        /// Automatically schedule announcements based on info provided in <see cref="Gravestone"/>.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="options">Specifies how <see cref="AnnouncementScheduler"/> should make a schedule based on <see cref="Gravestone"/> info.</param>
        public void Schedule(Gravestone gravestone, AnnouncementOptions? options = null)
        {
            options ??= AnnouncementOptions.Default;
            foreach (var futureDays in options.CriticalDays)
                Schedule(gravestone, gravestone.DateClose.AddDays(-futureDays));
        }

        /// <summary>
        /// Cancel all related scheduled announcements.
        /// </summary>
        /// <param name="gravestone">The <see cref="Gravestone"/> associated with corresponding announcements.</param>
        public void Cancel(Gravestone gravestone)
        {
            if (_scheduled.ContainsKey(gravestone))
            {
                // cancel all tasks
                _scheduled[gravestone].Cancel();
                // remove the pair from scheduled pool
                _scheduled.Remove(gravestone);
            }
            else throw new InvalidOperationException("The gravestone isn't registered yet!");
        }
    }
}
