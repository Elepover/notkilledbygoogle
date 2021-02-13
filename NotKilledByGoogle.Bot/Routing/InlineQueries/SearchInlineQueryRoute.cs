using System.Linq;
using System.Threading.Tasks;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries
{
    public class SearchInlineQueryRoute : IRoutable<BotRoutingArgs>
    {
        public bool IsEligible(BotRoutingArgs args)
            => !string.IsNullOrWhiteSpace(args.IncomingUpdate.InlineQuery.Query);

        public Task ProcessAsync(BotRoutingArgs args)
            => args.BotClient.AnswerInlineQueryAsync(
                inlineQueryId: args.IncomingUpdate.InlineQuery.Id,
                results: args.GraveKeeper.Gravestones
                    .Where(x =>
                        x.Name
                            .ToLowerInvariant()
                            .Contains(args.IncomingUpdate.InlineQuery.Query.ToLowerInvariant()))
                    .Select(InlineQueryResponseComposer.GetArticleResult)
                    .Take(50),
                isPersonal: false);
    }
}
