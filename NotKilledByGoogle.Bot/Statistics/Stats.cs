using System.Collections.Generic;
using System.Linq;

namespace NotKilledByGoogle.Bot.Statistics
{
    /// <summary>
    /// Representing a pack of statistics data.
    /// </summary>
    public class Stats
    {
        /// <summary>
        /// Registered statistics data for easy enumeration.
        /// </summary>
        public List<SafeIntTag> RegisteredSafeInts { get; } = new();
        
#pragma warning disable CS8618
        public Stats()
#pragma warning restore CS8618
        {
            GetType()
                .GetProperties()
                .Where(x => x.PropertyType == typeof(SafeInt))
                .ForEach(x =>
                {
                    var id = x.Name;
                    var displayName = id;
                    var newSafeInt = new SafeInt();
                    x.SetValue(this, newSafeInt);

                    var foundAttr =
                        x.CustomAttributes.FirstOrDefault(attrib => attrib.AttributeType == typeof(StatNameAttribute));
                    if (foundAttr is not null)
                        displayName = foundAttr.ConstructorArguments[0].Value?.ToString() ?? displayName;
                    RegisteredSafeInts.Add(new SafeIntTag(id, displayName, newSafeInt));
                });
        }

        public void Clear()
            => RegisteredSafeInts.ForEach(x => x.SafeInt.Zero());

        [StatName("Process errors")]
        public SafeInt ProcessErrors { get; set; }
        [StatName("Fetch errors")]
        public SafeInt FetchErrors { get; set; }
        [StatName("Processed inline queries")]
        public SafeInt InlineQueriesProcessed { get; set; }
        [StatName("Processed messages")]
        public SafeInt MessagesProcessed { get; set; }
        [StatName("Received updates")]
        public SafeInt UpdatesReceived { get; set; }
    }
}
