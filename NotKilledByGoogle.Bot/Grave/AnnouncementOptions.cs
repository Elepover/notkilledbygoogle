using System.Collections.Generic;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Specifies how <see cref="AnnouncementScheduler"/> should make a schedule based on <see cref="Gravestone"/> info.
    /// </summary>
    public class AnnouncementOptions
    {
        public AnnouncementOptions(IEnumerable<int> criticalDays)
        {
            CriticalDays = criticalDays;
        }

        /// <summary>
        /// Default option, where:<br />
        /// <see cref="CriticalDays"/> = {0}.
        /// </summary>
        public static readonly AnnouncementOptions Default = new (new []{0});
        
        /// <summary>
        /// Determines when should the <see cref="AnnouncementScheduler"/> make announcements.
        /// </summary>
        public IEnumerable<int> CriticalDays { get; }
    }
}