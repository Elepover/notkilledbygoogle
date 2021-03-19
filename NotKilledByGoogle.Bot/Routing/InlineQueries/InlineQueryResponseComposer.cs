using System;
using System.Collections.Generic;
using System.Linq;
using NotKilledByGoogle.Bot.Grave;
using NotKilledByGoogle.Bot.Routing.InlineQueries.Commands;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public static class InlineQueryResponseComposer
    {
        private const string SearchTipsDefaultTextContent = "Use this bot to search for literally any dead or dying product in the Google graveyard!";
        
        public static IEnumerable<InlineQueryResultArticle> AddSearchTips(
            this IEnumerable<InlineQueryResultArticle> results,
            ResultType resultType)
        {
            var list = results.ToList();
            string title = "", description = "";
            if (list.Any())
            {
                title = $"Search result ({(list.Count > 50 ? $"50 of {list.Count}" : list.Count)})";
                description = resultType switch
                {
                    ResultType.SearchShort => "Using short messages.",
                    ResultType.Ascending => "Using ascending sort by death date.",
                    ResultType.Descending => "Using descending sort by death date.",
                    ResultType.Oldest => "Searching by oldest product upon death.",
                    ResultType.Youngest => "Searching by youngest product upon death.",
                    _ => "Try adding /short, /ascend, /descend, /oldest or /youngest at the front."
                };
            }
            else
            {
                title = "No products found";
                description = "Try another search term.";
            }

            list.Insert(0,
                new(
                    $"search-result-{DateTimeOffset.UtcNow.UtcTicks}",
                    title,
                    new InputTextMessageContent(SearchTipsDefaultTextContent)
                )
                {
                    Description = description
                });

            return list;
        }

        public static IEnumerable<InlineQueryResultArticle> InvalidCommand(string commandName)
            => new[] {
                new InlineQueryResultArticle(
                $"search-result-{DateTimeOffset.UtcNow.UtcTicks}",
                "Invalid command",
                new InputTextMessageContent(SearchTipsDefaultTextContent)
            ) { Description = $"Command {commandName} not found." }
            };
        
        public static InlineQueryResultArticle GetArticleResult(Gravestone gravestone, bool useShortMessage = false)
            => new(
                $"{gravestone.DateClose:d}-{gravestone.Name}",
                $"{gravestone.Name}",
                new InputTextMessageContent(useShortMessage ? GetShortMessageText(gravestone) : GetMessageText(gravestone))
                {
                    ParseMode = ParseMode.Html,
                    DisableWebPagePreview = true
                })
            {
                Description = $"{(gravestone.DateClose < DateTimeOffset.UtcNow ? "Dead since" : "Dies on")} {gravestone.DateClose:ddd MMM dd, yyyy} ({(IsDead(gravestone) ? Utils.Age(DateTimeOffset.UtcNow - gravestone.DateClose) + " ago" : Utils.Age(gravestone.DateClose - DateTimeOffset.UtcNow) + " from now")}, {Utils.SimpleAge(gravestone.DateClose - gravestone.DateOpen)} old upon death)."
            };

        public static bool IsDead(Gravestone gravestone)
            => gravestone.DateClose < DateTimeOffset.UtcNow;

        public static string GetShortMessageText(Gravestone gravestone)
            => $"<b>{gravestone.Name}</b> {(IsDead(gravestone) ? "has <i>died</i> since" : "dies on")} {gravestone.DateClose:ddd MMM dd, yyyy}.";
        
        public static string GetMessageText(Gravestone gravestone)
            => $"<b>{gravestone.Name}</b>\n\n" + 
               $"📱 <b>Type</b>: {MessageFormatter.DeceasedTypeName(gravestone.DeceasedType).CapitalizeFirst()}\n" +
               $"🚀 <b>Launched</b>: {gravestone.DateOpen:ddd MMM dd, yyyy}\n" +
               $"⏱ <b>Fate</b>: {(IsDead(gravestone) ? "<i>Dead</i> since" : "Dies on")} {gravestone.DateClose:ddd MMM dd, yyyy}\n" +
               $"🗓 <b>Time {(IsDead(gravestone) ? "since" : "left")}</b>: {(IsDead(gravestone) ? Utils.Age(DateTimeOffset.UtcNow - gravestone.DateClose) : Utils.Age(gravestone.DateClose - DateTimeOffset.UtcNow))}\n" +
               $"⌛️ <b>Lifespan</b>: {Utils.Age(gravestone.DateClose - gravestone.DateOpen)}\n" +
               $"📝 <b>History</b>: {gravestone.Description}\n\n" +
               $"<a href=\"{gravestone.ReferenceLink}\">📰 <b>Reports</b></a>";
    }
}
