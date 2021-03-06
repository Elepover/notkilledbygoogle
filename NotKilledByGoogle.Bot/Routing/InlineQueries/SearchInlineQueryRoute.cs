using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class SearchInlineQueryRoute : IRoutable<BotRoutingContext>
    {
        public bool IsEligible(BotRoutingContext context)
            => !string.IsNullOrWhiteSpace(context.Update.InlineQuery.Query);

        public async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context.GraveKeeper.Gravestones
                .Where(x =>
                    x.Name
                        .ToLowerInvariant()
                        .Contains(context.Update.InlineQuery.Query.ToLowerInvariant()))
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips()
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
