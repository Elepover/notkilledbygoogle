using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Commands;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing.Extensions
{
    public static class BotRoutingArgsExtensions
    {
        /// <summary>
        /// Send text reply to a message. Only works when <see cref="UpdateType"/> is <see cref="UpdateType.Message"/>.
        /// </summary>
        public static Task ReplyTextMessageAsync(
            this BotRoutingContext context,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false)
            => context.BotClient.SendTextMessageAsync(
                context.Update.Message.Chat.Id,
                text,
                parseMode,
                disableWebPagePreview,
                disableNotification,
                context.Update.Message.MessageId
            );

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
    }
}
