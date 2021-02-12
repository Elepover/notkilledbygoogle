using System;
using NotKilledByGoogle.Bot.Grave;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public static class InlineQueryResponseParser
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
               $"📱 <b>Type</b>: {MessageFormatter.DeceasedTypeName(gravestone.DeceasedType)}\n" +
               $"🚀 <b>Launched on</b>: {gravestone.DateOpen:ddd MM dd, yyyy}\n" +
               $"⏱ <b>Status</b>: {(IsDead(gravestone) ? "<b>dead</b>" : "dying")} on {gravestone.DateClose:ddd MMM dd, yyyy}\n" +
               $"🗓 <b>Days {(IsDead(gravestone) ? "since" : "left")}</b>: {Math.Abs((DateTimeOffset.UtcNow - gravestone.DateClose).TotalDays):F1}\n" +
               $"⌛️ <b>Lifespan</b>: {Utils.Age(gravestone.DateClose - gravestone.DateOpen)}\n" +
               $"📝 <b>History</b>: {gravestone.Description}\n\n" +
               $"📰 <b>Reports</b>: <a href=\"{gravestone.ReferenceLink}\">link</a>";
    }
}
