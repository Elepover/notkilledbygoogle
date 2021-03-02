using NotKilledByGoogle.Bot.Config;
using NotKilledByGoogle.Bot.Grave;
using NotKilledByGoogle.Bot.Statistics;
using SimpleRouting.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing
{
    public class BotRoutingContext : IRoutingContext
    {
        public BotRoutingContext(ITelegramBotClient client, GraveKeeper graveKeeper, IConfigManager<BotConfig> configManager, Stats stats, Update update)
        {
            BotClient = client;
            GraveKeeper = graveKeeper;
            ConfigManager = configManager;
            Stats = stats;
            Update = update;
        }
        public ITelegramBotClient BotClient { get; }
        public GraveKeeper GraveKeeper { get; }
        public IConfigManager<BotConfig> ConfigManager { get; }
        public Stats Stats { get; }
        public RouteTarget Target { get; set; } = RouteTarget.Stop;
        public Update Update { get; }
    }
}
