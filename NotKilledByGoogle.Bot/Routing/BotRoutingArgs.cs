using SimpleRouting.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing
{
    public class BotRoutingArgs : IRoutingArgs
    {
        public BotRoutingArgs(ITelegramBotClient client, Update incomingUpdate)
        {
            BotClient = client;
            IncomingUpdate = incomingUpdate;
        }
        public ITelegramBotClient BotClient { get; }
        public bool Continue { get; set; }
        public Update IncomingUpdate { get; set; }
    }
}