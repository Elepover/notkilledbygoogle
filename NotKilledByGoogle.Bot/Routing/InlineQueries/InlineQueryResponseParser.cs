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
                gravestone.Name,
                new InputTextMessageContent(GetMessageText(gravestone))
                {
                    ParseMode = ParseMode.MarkdownV2,
                    DisableWebPagePreview = true
                });

        public static string GetMessageText(Gravestone gravestone)
            => Utils.EscapeIllegalMarkdownV2Chars(
                $"*{gravestone.Name}*\n\n" +
                $"📱 *Type*: {MessageFormatter.DeceasedTypeName(gravestone.DeceasedType)}\n" +
                $"⏱ *Status*: {(gravestone.DateClose < DateTimeOffset.UtcNow ? "*dead*" : "dying")} on {gravestone.DateClose:D}\n" +
                $"📝 *History*: {gravestone.Description}\n\n" +
                $"📰 *Reports*: [link]({gravestone.ReferenceLink})");
    }
}
