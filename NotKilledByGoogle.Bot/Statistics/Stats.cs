namespace NotKilledByGoogle.Bot.Statistics
{
#pragma warning disable 8618
    // ALSO IGNORE SETTER WARNING
    // WE TOOK CARE OF THEM IN CONSTRUCTOR
    
    /// <summary>
    /// Representing a pack of statistics data.
    /// </summary>
    public class Stats : StatsBase
    {
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
#pragma warning restore 8618
}
