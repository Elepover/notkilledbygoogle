using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Routing
{
    public abstract class Router<TIncoming> : IRoutable<TIncoming>
    {
        /// <summary>
        /// Registered routes in this <see cref="Router{TIncoming}"/>
        /// </summary>
        public LinkedList<IRoutable<TIncoming>> RegisteredRoutes { get; } = new();

        public virtual bool IsEligible(TIncoming incoming) =>
            RegisteredRoutes.Any(route => route.IsEligible(incoming));

        public virtual Task ProcessAsync(IRoutingArgs<TIncoming> args)
            => RouteAsync(args.IncomingData);

        public virtual async Task RouteAsync(TIncoming incoming)
        {
            foreach (var route in RegisteredRoutes)
            {
                var args = new BasicRoutingArgs<TIncoming>(incoming);
                await route.ProcessAsync(args);
                if (!args.Continue) break;
            }
        }
    }
}
