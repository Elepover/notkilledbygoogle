using System.Threading.Tasks;

namespace NotKilledByGoogle.Bot.Routing.Commands
{
    public class StartCommand : CommandRouteBase
    {
        public override string Prefix => "start";
        
        public override Task ProcessAsync(BotRoutingArgs args)
            => args.BotClient.SendTextMessageAsync(
                args.IncomingUpdate.Message.Chat.Id,
                "Hello! Private message features are not developed yet but stay tuned there!");
    }
}
