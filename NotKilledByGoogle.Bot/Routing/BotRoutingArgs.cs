using NotKilledByGoogle.Bot.Config;
using NotKilledByGoogle.Bot.Grave;
using SimpleRouting.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing
{
    public class BotRoutingArgs : IRoutingArgs
    {
        public BotRoutingArgs(ITelegramBotClient client, GraveKeeper graveKeeper, IConfigManager<BotConfig> configManager, Update incomingUpdate)
        {
            BotClient = client;
            GraveKeeper = graveKeeper;
            ConfigManager = configManager;
            IncomingUpdate = incomingUpdate;
        }
        public ITelegramBotClient BotClient { get; }
        public GraveKeeper GraveKeeper { get; }
        public IConfigManager<BotConfig> ConfigManager { get; }
        public bool Continue { get; set; } = false;
        public Update IncomingUpdate { get; set; }
    }
}
