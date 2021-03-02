using System.Threading.Tasks;
using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container a message.
    /// </summary>
    public class MessageRouter : Router<BotRoutingContext>
    {
        public override bool IsEligible(BotRoutingContext context)
            => context.Update.Type == UpdateType.Message;

        public override Task ProcessAsync(BotRoutingContext context)
        {
            context.Stats.MessagesProcessed.Inc();
            return base.ProcessAsync(context);
        }
    }
}
