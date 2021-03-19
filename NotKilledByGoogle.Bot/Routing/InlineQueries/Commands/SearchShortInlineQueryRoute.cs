using System.Linq;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public class SearchShortInlineQueryRoute : InlineQueryCommandBase
    {
        public override string Prefix => "short";

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            var results = context
                .SearchByCommand()
                .Select(x => InlineQueryResponseComposer.GetArticleResult(x, true))
                .AddSearchTips(ResultType.SearchShort)
                .Take(50);
            context.RecordGenerationSegmentTime();
            await context.AnswerInlineQueryAsync(results);
        }
    }
}
