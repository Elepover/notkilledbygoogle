using System;

namespace NotKilledByGoogle.Bot.Statistics
{
    [AttributeUsage(AttributeTargets.Property)]
    public class StatNameAttribute : Attribute
    {
        public StatNameAttribute(string statName)
        {
            StatName = statName;
        }
        
        public string StatName { get; }
    }
}