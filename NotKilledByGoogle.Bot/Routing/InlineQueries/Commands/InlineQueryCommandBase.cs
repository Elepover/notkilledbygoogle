using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public abstract class InlineQueryCommandBase: IRoutable<BotRoutingContext>
    {
        /// <summary>
        /// Command prefix in lower case, without leading <c>'/'</c>.
        /// </summary>
        public abstract string Prefix { get; }
        
        /// <summary>
        /// Process the route.
        /// </summary>
        /// <param name="context">Incoming <see cref="BotRoutingContext"/>.</param>
        public abstract Task ProcessAsync(BotRoutingContext context);

        /// <summary>
        /// Check if the command route can handle this command. Prefix is not case-sensitive.
        /// </summary>
        /// <param name="context">Incoming <see cref="BotRoutingContext"/>.</param>
        public bool IsEligible(BotRoutingContext context)
            => context.GetInlineQuery().ToLowerInvariant().StartsWith("/" + Prefix) == true;
    }
}
