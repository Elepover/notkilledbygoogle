using System.Threading.Tasks;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.Commands
{
    public abstract class CommandRouteBase : IRoutable<BotRoutingContext>
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
            => context.Update.Message?.Text?.ToLowerInvariant().StartsWith("/" + Prefix) ?? false;
    }
}
