using System;
using System.Collections.Generic;
using System.Linq;

namespace NotKilledByGoogle.Bot.Statistics
{
    public class Stats
    {
        public Dictionary<string, SafeInt> RegisteredSafeInts { get; } = new();
        
#pragma warning disable CS8618
        public Stats()
#pragma warning restore CS8618
        {
            GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(SafeInt))
                .ForEach(x =>
                {
                    var newSafeInt = new SafeInt();
                    x.SetValue(this, newSafeInt);
                    RegisteredSafeInts.Add(x.Name, newSafeInt);
                });
        }

        public void Clear()
            => RegisteredSafeInts.ForEach(x => x.Value.Zero());

        public SafeInt ProcessErrors { get; set; }
        public SafeInt FetchErrors { get; set; }
        public SafeInt InlineQueriesProcessed { get; set; }
        public SafeInt MessagesProcessed { get; set; }
        public SafeInt UpdatesReceived { get; set; }
    }
}
