using System.Threading.Tasks;
using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container an inline query.
    /// </summary>
    public class InlineQueryRouter : Router<BotRoutingContext>
    {
        public override bool IsEligible(BotRoutingContext context)
            => context.Update.Type == UpdateType.InlineQuery;

        public override Task ProcessAsync(BotRoutingContext context)
        {
            context.Stats.InlineQueriesProcessed.Inc();
            return base.ProcessAsync(context);
        }
    }
}
