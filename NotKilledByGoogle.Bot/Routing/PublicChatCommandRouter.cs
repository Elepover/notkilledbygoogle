using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container a command (routed by <see cref="MessageRouter"/>).
    /// </summary>
    public class PublicChatCommandRouter : Router<BotRoutingArgs>
    {
        public override bool IsEligible(BotRoutingArgs args)
            => args.IncomingUpdate.Type == UpdateType.Message &&
               (args.IncomingUpdate.Message.Chat.Type == ChatType.Group || args.IncomingUpdate.Message.Chat.Type == ChatType.Supergroup) &&
               (args.IncomingUpdate.Message.Text?.StartsWith('/') ?? false);
    }
}
