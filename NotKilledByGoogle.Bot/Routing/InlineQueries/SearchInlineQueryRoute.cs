using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using NotKilledByGoogle.Bot.Routing.InlineQueries.Commands;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class SearchInlineQueryRoute : IRoutable<BotRoutingContext>
    {
        public bool IsEligible(BotRoutingContext context)
            => !string.IsNullOrWhiteSpace(context.GetInlineQuery());

        public async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context
                .SearchByFullQuery()
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips(ResultType.Search)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
