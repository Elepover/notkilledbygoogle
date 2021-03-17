using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Commands;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;

namespace NotKilledByGoogle.Bot.Routing.Extensions
{
    public static class BotRoutingContextExtensions
    {
        /// <summary>
        /// Send text reply to a message. Only works when <see cref="UpdateType"/> is <see cref="UpdateType.Message"/>.
        /// </summary>
        public static async Task ReplyTextMessageAsync(
            this BotRoutingContext context,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false)
        {
            context.EnsureSegment();  
            await context.BotClient.SendTextMessageAsync(
                context.Update.Message.Chat.Id,
                text,
                parseMode,
                disableWebPagePreview,
                disableNotification,
                context.Update.Message.MessageId
            );
            context.RecordSendingSegmentTime();
        }

        /// <summary>
        /// Answer to an inline query request. Only works when <see cref="UpdateType"/> is <see cref="UpdateType.InlineQuery"/>.
        /// </summary>
        public static async Task AnswerInlineQueryAsync(
            this BotRoutingContext context,
            IEnumerable<InlineQueryResultBase> results)
        {
            context.EnsureSegment();
            await context.BotClient.AnswerInlineQueryAsync(
                inlineQueryId: context.Update.InlineQuery.Id,
                results: results,
                isPersonal: false
            );
            context.RecordSendingSegmentTime();
        }

        /// <summary>
        /// Get message sender's Telegram ID.
        /// </summary>
        public static long GetSenderId(this BotRoutingContext context)
            => context.Update.Message.From.Id;

        /// <summary>
        /// Checks if the sender is an administrator.
        /// </summary>
        public static bool IsFromAdmin(this BotRoutingContext context)
            => context.ConfigManager.Config.AdminIds.Contains(context.GetSenderId());

        /// <summary>
        /// Checks if the sender is a bot. 
        /// </summary>
        public static bool IsFromBot(this BotRoutingContext context)
            => context.Update.Message.From.IsBot;

        /// <summary>
        /// Checks if the sender is a real person.
        /// </summary>
        public static bool IsFromRealPerson(this BotRoutingContext context)
            => !context.IsFromBot();

        /// <summary>
        /// Get sender classification flags.
        /// </summary>
        public static SenderClassification GetSenderClassification(this BotRoutingContext context)
        {
            var result = SenderClassification.None;

            if (context.IsFromBot()) result |= SenderClassification.Bot;
            if (context.IsFromRealPerson()) result |= SenderClassification.User;
            if (context.IsFromAdmin()) result |= SenderClassification.Admin;

            return result;
        }
        
        /// <summary>
        /// Ensures that the <see cref="BotRoutingContext.ProcessTimes"/> has at least one segment.
        /// </summary>
        public static void EnsureSegment(this BotRoutingContext context)
        {
            if (context.ProcessTimes.Count == 0)
                context.RecordGenerationSegmentTime();
        }
        
        /// <summary>
        /// Records a segment as response generation is complete.
        /// </summary>
        public static void RecordGenerationSegmentTime(this BotRoutingContext context) => context.RecordSegmentTime("gen");
        
        /// <summary>
        /// Records a segment as message sending is complete.
        /// </summary>
        public static void RecordSendingSegmentTime(this BotRoutingContext context) => context.RecordSegmentTime("net");
    }
}
