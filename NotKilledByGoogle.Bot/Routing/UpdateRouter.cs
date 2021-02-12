using System.Threading.Tasks;
using SimpleRouting.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> object.
    /// </summary>
    public class UpdateRouter : Router<BotRoutingArgs>
    {
        public UpdateRouter(ITelegramBotClient client)
        {
            _botClient = client;
        }
        
        private ITelegramBotClient _botClient;

        public override Task ProcessAsync(BotRoutingArgs args)
            => RouteAsync(new BotRoutingArgs(_botClient, args.IncomingUpdate));
    }
}
