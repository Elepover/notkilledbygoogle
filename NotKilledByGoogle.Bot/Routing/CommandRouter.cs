using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container a command (routed by <see cref="MessageRouter"/>).
    /// </summary>
    public class CommandRouter : Router<Update, BotRoutingArgs>
    {
        public override bool IsEligible(Update incoming)
            => incoming.Type == UpdateType.Message &&
               (incoming.Message.Text?.StartsWith('/') ?? false);
    }
}
