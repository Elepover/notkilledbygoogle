using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Config;
using NotKilledByGoogle.Bot.Grave;
using SimpleRouting.Routing;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace NotKilledByGoogle.Bot.Routing
{
    /// <summary>
    /// A router to route an <see cref="Update"/> object.
    /// </summary>
    public class UpdateRouter : Router<BotRoutingContext>
    {
        public UpdateRouter(ITelegramBotClient client, GraveKeeper graveKeeper, IConfigManager<BotConfig> configManager)
        {
            _botClient = client;
            _graveKeeper = graveKeeper;
            _configManager = configManager;
        }
        
        private ITelegramBotClient _botClient;
        private GraveKeeper _graveKeeper;
        private IConfigManager<BotConfig> _configManager;

        public Task RouteAsync(Update update)
            => RouteAsync(new BotRoutingContext(
                _botClient,
                _graveKeeper,
                _configManager,
                update
            ));
    }
}
