using System;
using System.Text.RegularExpressions;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class CommandInlineQuery
    {
        private static readonly Regex CommandMatch = new("^\\/(?! )[a-zA-Z ]+");

        public static bool IsValid(string query)
            => CommandMatch.IsMatch(query);

        public CommandInlineQuery(string raw)
        {
            if (!CommandMatch.IsMatch(raw)) throw new ArgumentException("Inline query does not contain a command.");
            Raw = raw;
        }
        
        public string Raw { get; }
        public string Command
        {
            get
            {
                var slashIndex = Raw.IndexOf('/');
                var spaceIndex = Raw.IndexOf(' ', slashIndex);
                if (spaceIndex == -1)
                    spaceIndex = Raw.Length;
                    
                return Raw.Substring(slashIndex, spaceIndex - slashIndex);
            }
        }

        public string Query => Raw.Replace(Command, string.Empty).TrimStart(' ').TrimEnd(' ');
    }
}