using System.Linq;
using System.Threading.Tasks;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class DefaultInlineQueryRoute : IRoutable<BotRoutingContext>
    {
        public bool IsEligible(BotRoutingContext context)
            => string.IsNullOrWhiteSpace(context.Update.InlineQuery.Query);

        public Task ProcessAsync(BotRoutingContext context)
            => context.BotClient.AnswerInlineQueryAsync(
                inlineQueryId: context.Update.InlineQuery.Id,
                results: context.GraveKeeper.Gravestones
                    .Select(InlineQueryResponseComposer.GetArticleResult)
                    .Take(50),
                isPersonal: false);
    }
}
