using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container a command (routed by <see cref="MessageRouter"/>).
    /// </summary>
    public class PrivateChatCommandRouter : Router<BotRoutingContext>
    {
        public override bool IsEligible(BotRoutingContext context)
            => context.Update.Type == UpdateType.Message &&
               context.Update.Message.Chat.Type == ChatType.Private &&
               (context.Update.Message.Text?.StartsWith('/') ?? false);
    }
}
