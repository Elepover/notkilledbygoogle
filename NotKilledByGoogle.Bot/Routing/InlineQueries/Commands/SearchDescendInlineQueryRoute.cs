using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public class SearchDescendInlineQueryRoute : InlineQueryCommandBase
    {
        public override string Prefix => "descend";

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context
                .SearchByCommand()
                .OrderByDescending(x => x.DateClose)
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips(ResultType.Descending)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
