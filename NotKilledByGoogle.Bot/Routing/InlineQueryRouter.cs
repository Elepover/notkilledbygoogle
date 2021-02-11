using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container an inline query.
    /// </summary>
    public class InlineQueryRouter : Router<Update, BotRoutingArgs>
    {
        public override bool IsEligible(Update incoming)
            => incoming.Type == UpdateType.InlineQuery;
    }
}
