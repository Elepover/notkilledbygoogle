using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public class SearchYoungestInlineQueryRoute : InlineQueryCommandBase
    {
        public override string Prefix => "youngest";

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context
                .SearchByCommand()
                .OrderBy(x => x.DateClose - x.DateOpen)
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips(ResultType.Youngest)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
