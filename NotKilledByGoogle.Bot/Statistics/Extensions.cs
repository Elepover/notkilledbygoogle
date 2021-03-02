using System;
using System.Collections.Generic;

namespace NotKilledByGoogle.Bot.Statistics
{
    public static class Extensions
    {
        public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
        {
            foreach (var obj in source)
                action(obj);
        }
    }
}
