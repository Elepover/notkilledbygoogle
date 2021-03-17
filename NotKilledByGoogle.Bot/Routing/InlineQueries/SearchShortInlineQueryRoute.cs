using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class SearchShortInlineQueryRoute : IRoutable<BotRoutingContext>
    {
        public bool IsEligible(BotRoutingContext context)
            => !string.IsNullOrWhiteSpace(context.Update.InlineQuery.Query) &&
               context.Update.InlineQuery.Query.ToLowerInvariant().Contains("/short");

        public async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context.GraveKeeper.Gravestones
                .Where(x =>
                    x.Name
                        .ToLowerInvariant()
                        .Contains(context.Update.InlineQuery.Query
                            .ToLowerInvariant()
                            .Replace("/short", "")
                            .TrimEnd(' ')
                            .TrimStart(' ')))
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x, true))
                .AddSearchTips(true)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}