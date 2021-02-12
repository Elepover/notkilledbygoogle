using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.Commands.PrivateChatCommands
{
    public class StartCommand : CommandRouteBase
    {
        public override string Prefix => "start";

        public override Task ProcessAsync(BotRoutingArgs args)
            => args.ReplyTextMessageAsync(
                "Hello! Command features are not developed yet but stay tuned there!");
    }
}
