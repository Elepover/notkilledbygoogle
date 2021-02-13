using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.Commands.PrivateChatCommands
{
    public class StartCommand : CommandRouteBase
    {
        public override string Prefix => "start";

        public override Task ProcessAsync(BotRoutingContext context)
            => context.ReplyTextMessageAsync(
                "Hello! Command features are not developed yet but stay tuned there!");
    }
}
