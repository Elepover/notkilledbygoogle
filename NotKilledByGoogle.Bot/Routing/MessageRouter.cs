using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container a message.
    /// </summary>
    public class MessageRouter : Router<BotRoutingArgs>
    {
        public override bool IsEligible(BotRoutingArgs args)
            => args.IncomingUpdate.Type == UpdateType.Message;
    }
}
