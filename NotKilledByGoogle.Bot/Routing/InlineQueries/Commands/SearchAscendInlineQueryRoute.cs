using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public class SearchAscendInlineQueryRoute : InlineQueryCommandBase
    {
        public override string Prefix => "ascend";

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context
                .SearchByCommand()
                .OrderBy(x => x.DateClose)
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x))
                .AddSearchTips(ResultType.Ascending)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
