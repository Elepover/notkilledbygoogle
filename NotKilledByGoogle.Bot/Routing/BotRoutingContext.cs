using System;
using System.Collections.Generic;
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

        private readonly DateTimeOffset _timeCreated = DateTimeOffset.UtcNow;
        
        public ITelegramBotClient BotClient { get; }
        public GraveKeeper GraveKeeper { get; }
        public IConfigManager<BotConfig> ConfigManager { get; }
        public Stats Stats { get; }
        public RouteTarget Target { get; set; } = RouteTarget.Stop;
        public Update Update { get; }
        public List<TimingSegment> ProcessTimes { get; } = new();
        
        /// <summary>
        /// Record a timing segment.
        /// </summary>
        /// <param name="segmentName">Segment name.</param>
        public void RecordSegmentTime(string segmentName)
        {
            if (ProcessTimes.Count > 0)
                ProcessTimes.Add(new TimingSegment(segmentName, DateTimeOffset.UtcNow - ProcessTimes[^1].TimeStamp));
            else
                ProcessTimes.Add(new TimingSegment(segmentName, DateTimeOffset.UtcNow - _timeCreated));
        }
    }
}
