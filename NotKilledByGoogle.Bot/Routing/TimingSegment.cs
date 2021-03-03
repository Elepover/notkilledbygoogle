using System;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// Represents a time segment in routing.
    /// </summary>
    public class TimingSegment
    {
        public TimingSegment(string segmentName, TimeSpan duration)
        {
            SegmentName = segmentName;
            Duration = duration;
        }
        
        public string SegmentName { get; }
        public DateTimeOffset TimeStamp { get; } = DateTimeOffset.UtcNow;
        public TimeSpan Duration { get; }
    }
}