using System;
using System.Threading.Tasks;
using NotKilledByGoogle.Bot.Routing.Extensions;

namespace NotKilledByGoogle.Bot.Routing.Commands.PrivateChatCommands
{
    public class StartCommand : CommandRouteBase
    {
        public override string Prefix => "start";

        public override SenderClassification[] AcceptedFrom => new[]
        {
            SenderClassification.Admin,
            SenderClassification.User
        };

        public override async Task ProcessAsync(BotRoutingContext context)
        {
            if (context.IsFromAdmin())
            {
                await context.ReplyTextMessageAsync($"Hello!\nBot version is {Program.Version}.\nUptime: {DateTime.UtcNow:hh:mm}, up {Program.AppStopwatch.Elapsed:d day(s), hh:mm:ss}");
            }
            else
            {
                await context.ReplyTextMessageAsync("Hello! Command features are not developed yet but stay tuned there!");
            }
        }
    }
}
