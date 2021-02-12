using System.Threading.Tasks;
using SimpleRouting.Routing;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing.Commands
{
    public abstract class CommandRouteBase : IRoutable<BotRoutingArgs>
    {
        /// <summary>
        /// Command prefix in lower case, without leading <c>'/'</c>.
        /// </summary>
        public abstract string Prefix { get; }

        /// <summary>
        /// Check if the command route can handle this command. Prefix is not case-sensitive.
        /// </summary>
        /// <param name="args">Incoming <see cref="BotRoutingArgs"/>.</param>
        public bool IsEligible(BotRoutingArgs args)
            => args.IncomingUpdate.Message?.Text?.ToLowerInvariant().StartsWith("/" + Prefix) ?? false;

        public abstract Task ProcessAsync(BotRoutingArgs args);
    }
}
