using System;
using System.Collections.Generic;
using System.Linq;
using NotKilledByGoogle.Bot.Grave;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public static class InlineQueryResponseComposer
    {
        private const string SearchTipsDefaultTextContent = "Use this bot to search for literally any dead or dying product in the Google graveyard!";
        
        public static IEnumerable<InlineQueryResultArticle> AddSearchTips(
            this IEnumerable<InlineQueryResultArticle> results,
            bool usingShortMessage = false)
        {
            var list = results.ToList();
            string title = "", description = "";
            if (list.Any())
            {
                title = $"Search result ({(list.Count > 50 ? $"50 of {list.Count}" : list.Count)})";
                description = usingShortMessage ?
                    "Using short messages." :
                    "Add /short in query to generate shorter messages.";
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

        public static IEnumerable<InlineQueryResultArticle> AddDefaultTips(
            this IEnumerable<InlineQueryResultArticle> results)
        {
            var list = results.ToList();
            
            list.Insert(0,
                new(
                    $"search-result-{DateTimeOffset.UtcNow.UtcTicks}",
                    $"Google graveyard search",
                    new InputTextMessageContent(SearchTipsDefaultTextContent)
                )
                {
                    Description = $"Showing most recent 50 gravestones (max). {list.Count} in total."
                });

            return list;
        }
        
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
                Description = $"{(gravestone.DateClose < DateTimeOffset.UtcNow ? "Dead since" : "Dies on")} {gravestone.DateClose:ddd MMM dd, yyyy} ({(IsDead(gravestone) ? Utils.Age(DateTimeOffset.UtcNow - gravestone.DateClose) + " ago" : Utils.Age(gravestone.DateClose - DateTimeOffset.UtcNow) + " later")})."
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
