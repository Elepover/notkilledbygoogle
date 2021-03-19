using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using NotKilledByGoogle.Bot.Routing.InlineQueries.Commands;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class DefaultInlineQueryRoute : IRoutable<BotRoutingContext>
    {
        public bool IsEligible(BotRoutingContext context)
            => string.IsNullOrWhiteSpace(context.Update.InlineQuery.Query);

        public async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context.GraveKeeper.Gravestones
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips(ResultType.Default)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
