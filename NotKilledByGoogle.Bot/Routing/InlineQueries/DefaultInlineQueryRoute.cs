using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
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
                .Select(InlineQueryResponseComposer.GetArticleResult)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.BotClient.AnswerInlineQueryAsync(
                inlineQueryId: context.Update.InlineQuery.Id,
                results: results,
                isPersonal: false);
            context.RecordSendingSegmentTime();
        }
    }
}
