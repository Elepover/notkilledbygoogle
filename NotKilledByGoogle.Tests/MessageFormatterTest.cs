using System;
using System.Collections.Generic;
using NotKilledByGoogle.Bot;
using Xunit;

namespace NotKilledByGoogle.Tests
{
    public class MessageFormatterTest
    {
        private static IEnumerable<object[]> FormatTimeLeftData()
        {
            yield return new object[] { TimeSpan.FromMinutes(10), "very soon" };
            yield return new object[] { TimeSpan.FromMinutes(119), "very soon" };
            yield return new object[] { TimeSpan.FromDays(1), "tomorrow" };
            yield return new object[] { TimeSpan.FromDays(1.4), "in 2 days" };
            yield return new object[] { TimeSpan.FromDays(1.5), "in 2 days" };
            yield return new object[] { TimeSpan.FromDays(1.9), "in 2 days" };
            yield return new object[] { TimeSpan.FromDays(2), "in 2 days" };
            yield return new object[] { TimeSpan.FromDays(2.4), "in 3 days" };
            yield return new object[] { TimeSpan.FromDays(2.9), "in 3 days" };
            yield return new object[] { TimeSpan.FromDays(3), "in 3 days" };
        }

        [Theory]
        [MemberData(nameof(FormatTimeLeftData))]
        public void FormatTimeLeft(TimeSpan left, string expected)
            => Assert.Equal(expected, MessageFormatter.FormatTimeLeft(left));
    }
}