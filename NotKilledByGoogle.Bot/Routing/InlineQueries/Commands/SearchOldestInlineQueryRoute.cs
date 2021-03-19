using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public class SearchOldestInlineQueryRoute : InlineQueryCommandBase
    {
        public override string Prefix => "oldest";

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context
                .SearchByCommand()
                .OrderByDescending(x => x.DateClose - x.DateOpen)
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips(ResultType.Oldest)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
