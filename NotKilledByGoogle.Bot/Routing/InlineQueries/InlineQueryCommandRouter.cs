using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using SimpleRouting.Routing;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    /// <summary>
    /// A router to route an <see cref="Update"/> container an inline query that has commands inside.
    /// </summary>
    public class InlineQueryCommandRouter : Router<BotRoutingContext>
    {
        public override bool IsEligible(BotRoutingContext context)
            => context.Update.Type == UpdateType.InlineQuery &&
               CommandInlineQuery.IsValid(context.GetInlineQuery());
    }
}
