using System.Threading.Tasks;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing.Extensions
{
    public static class BotRoutingArgsExtensions
    {
        /// <summary>
        /// Send text reply to a message. Only works when <see cref="UpdateType"/> is <see cref="UpdateType.Message"/>.
        /// </summary>
        public static Task ReplyTextMessageAsync(
            this BotRoutingArgs args,
            string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false)
            => args.BotClient.SendTextMessageAsync(
                args.IncomingUpdate.Message.Chat.Id,
                text,
                parseMode,
                disableWebPagePreview,
                disableNotification,
                args.IncomingUpdate.Message.MessageId
            );
    }
}
