using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Commands;
using Telegram.Bot.Types.Enums;

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
        /// Get message sender's Telegram ID.
        /// </summary>
        /// <returns></returns>
        public static long GetSenderId(this BotRoutingContext context)
            => context.Update.Message.From.Id;

        public static bool IsFromAdmin(this BotRoutingContext context)
            => context.ConfigManager.Config.AdminIds.Contains(context.GetSenderId());

        public static bool IsFromBot(this BotRoutingContext context)
            => context.Update.Message.From.IsBot;

        public static bool IsFromRealPerson(this BotRoutingContext context)
            => !context.IsFromBot();

        public static SenderClassification GetSenderClassification(this BotRoutingContext context)
        {
            var result = SenderClassification.None;

            if (context.IsFromBot()) result |= SenderClassification.Bot;
            if (context.IsFromRealPerson()) result |= SenderClassification.User;
            if (context.IsFromAdmin()) result |= SenderClassification.Admin;

            return result;
        }
        
        public static void EnsureSegment(this BotRoutingContext context)
        {
            if (context.ProcessTimes.Count == 0)
                context.RecordGenerationSegmentTime();
        }
        public static void RecordGenerationSegmentTime(this BotRoutingContext context) => context.RecordSegmentTime("g");
        public static void RecordSendingSegmentTime(this BotRoutingContext context) => context.RecordSegmentTime("s");
    }
}
