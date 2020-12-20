using System;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides methods for scheduling announcements.
    /// </summary>
    public class AnnouncementScheduler
    {
        public delegate void AnnouncementEventHandler(object? sender, AnnouncementEventArgs e);
        /// <summary>
        /// Raised when an announcement is made.
        /// </summary>
        public event AnnouncementEventHandler? Announcement;
        
        /// <summary>
        /// Schedule an announcement.
        /// </summary>
        /// <param name="gravestone">Corresponding <see cref="Gravestone"/>.</param>
        /// <param name="timeout">How long should the <see cref="AnnouncementScheduler"/> wait until time is up.</param>
        public void Schedule(Gravestone gravestone, TimeSpan timeout)
        {
            // TODO: implement it!
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
            foreach (var future in options.CriticalDays)
                Schedule(gravestone, gravestone.DateClose.AddDays(-future));
        }

        /// <summary>
        /// Cancel all related scheduled announcements.
        /// </summary>
        /// <param name="gravestone">The <see cref="Gravestone"/> associated with corresponding announcements.</param>
        public void Cancel(Gravestone gravestone)
        {
            // TODO: implement it!
        }
    }
}
