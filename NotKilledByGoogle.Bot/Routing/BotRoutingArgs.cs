using SimpleRouting.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing
{
    public class BotRoutingArgs : IRoutingArgs<Update>
    {
        public BotRoutingArgs(ITelegramBotClient client, Update incomingData)
        {
            BotClient = client;
            IncomingData = incomingData;
        }
        public ITelegramBotClient BotClient { get; }
        public bool Continue { get; set; }
        public Update IncomingData { get; set; }
    }
}