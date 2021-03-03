namespace NotKilledByGoogle.Bot.Statistics
{
    /// <summary>
    /// Used to keep track of a <see cref="SafeInt"/> in <see cref="Stats"/>.
    /// </summary>
    public class SafeIntTag
    {
        public SafeIntTag(string id, string displayName, SafeInt safeInt)
        {
            Id = id;
            DisplayName = displayName;
            SafeInt = safeInt;
        }
        
        public string Id { get; }
        public string DisplayName { get; }
        public SafeInt SafeInt { get; }
    }
}