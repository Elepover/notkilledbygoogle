using System;
using NotKilledByGoogle.Bot.Grave;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public static class InlineQueryResponseComposer
    {
        public static InlineQueryResultArticle GetArticleResult(Gravestone gravestone)
            => new(
                $"{gravestone.DateClose:d}-{gravestone.Name}",
                $"{gravestone.Name} ({(gravestone.DateClose < DateTimeOffset.UtcNow ? "dead" : "dying")} on {gravestone.DateClose:d})",
                new InputTextMessageContent(GetMessageText(gravestone))
                {
                    ParseMode = ParseMode.Html,
                    DisableWebPagePreview = true
                });

        public static bool IsDead(Gravestone gravestone)
            => gravestone.DateClose < DateTimeOffset.UtcNow;

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
