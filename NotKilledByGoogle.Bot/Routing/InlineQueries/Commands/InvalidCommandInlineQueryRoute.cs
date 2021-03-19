using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;
using SimpleRouting.Routing;

namespace NotKilledByGoogle.Bot.Routing.InlineQueries.Commands
{
    public class InvalidCommandInlineQueryRoute : IRoutable<BotRoutingContext>
    {
        public bool IsEligible(BotRoutingContext context) => CommandInlineQuery.IsValid(context.GetInlineQuery());

        public Task ProcessAsync(BotRoutingContext context)
            => context.AnswerInlineQueryAsync(
                InlineQueryResponseComposer.InvalidCommand(
                    new CommandInlineQuery(
                        context.GetInlineQuery()
                        ).Command
                    )
                );
    }
}
