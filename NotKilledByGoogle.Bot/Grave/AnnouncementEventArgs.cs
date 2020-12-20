using System;

namespace NotKilledByGoogle.Bot.Grave
{
    /// <summary>
    /// Provides data for <see cref="AnnouncementScheduler.Announcement"/> event.
    /// </summary>
    public class AnnouncementEventArgs : EventArgs
    {
        public AnnouncementEventArgs(Gravestone gravestone)
        {
            Gravestone = gravestone;
        }
        
        /// <summary>
        /// The data associated with the announcement.
        /// </summary>
        public Gravestone Gravestone { get; }
    }
}
